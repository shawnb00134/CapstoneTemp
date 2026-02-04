#!/usr/bin/env bash
set -euo pipefail

help=false

pgUser="${PG_USER:-}"
pgHost="${PG_HOST:-}"
pgPort="${PG_PORT:-5432}"
pgSslMode="${PG_SSLMODE:-require}"
SUBSCRIPTION_ID="<SUBSCRIPTION-ID>"  # Set to a real subscription ID to skip interactive selection

azResourceGroup=""
azServerName=""
fwRulePrefix="allow-current-ip"

teamName=""     # e.g. "team"
teamCount=0     # e.g. 2 -> team01, team02

print_help() {
  cat <<'EOF'
Usage: ./db-team-setup-azure.sh [options]

Options:
  --help                   Display this help message
  --pgUser <user>          Azure PostgreSQL AAD user/UPN (or set PG_USER)
  --pgHost <host>          Azure PostgreSQL host (or set PG_HOST)
                           e.g. myserver.postgres.database.azure.com
  --pgPort <port>          PostgreSQL port (default/env PG_PORT, default 5432)
  --pgSslMode <mode>       PostgreSQL sslmode (default/env PG_SSLMODE, default: require)
  -g, --resource-group <rg>    Azure resource group of the Postgres server
  -s, --server-name <name>     Azure Postgres server name
                           e.g. psql-uwg-cs-dev (FQDN is psql-uwg-cs-dev.postgres.database.azure.com)
  --fw-rule-prefix <name>  Prefix for firewall rule name (default: allow-current-ip)
  --team-name <base>       Base name for per-team DBs/roles (e.g. "team")
  --team-count <n>         How many teams to create (e.g. 2 -> team01, team02)

Behavior:
- Requires Azure CLI and an active 'az login'.
- Adds/updates a firewall rule allowing the current public IP
  on the specified Azure PostgreSQL Flexible Server.
- Acquires an Azure AD access token and uses it as the Postgres password.
- For each i in [1..team-count], we create:
    DBs:  <team-name><NN>_dev   and   <team-name><NN>_test
          (NN = zero-padded index, e.g. 01)
    Role: <team-name><NN>          (team-level group role, NOLOGIN)
    Role: <team-name><NN>_apiuser  (per-team role, NOLOGIN here; app can map logins as needed)
- schema/initdb.sql is run on the postgres DB to create each new DB and grant roles.
- schema/initschema.sql is run on each per-team DB to initialize schema/permissions.
- No migrations or finalize scripts are run; teams handle migrations themselves.

Environment overrides:
  PG_USER, PG_HOST, PG_PORT, PG_SSLMODE

Prereqs:
- psql in PATH
- Azure CLI (az) in PATH
- Valid az login (user, service principal, or managed identity).
EOF
}

# Azure helpers
require_az_login() {
  if ! command -v az >/dev/null 2>&1; then
    echo "ERROR: Azure CLI 'az' is not installed or not in PATH."
    exit 1
  fi

  if ! az account show >/dev/null 2>&1; then
    echo "ERROR: You are not logged into Azure (az account show failed)."
    echo "Please run: az login"
    exit 1
  fi
}

get_public_ip() {
  if command -v curl >/dev/null 2>&1; then
    curl -s https://ifconfig.me || curl -s https://api.ipify.org
    return
  fi

  if command -v dig >/dev/null 2>&1; then
    dig +short myip.opendns.com @resolver1.opendns.com
    return
  fi

  echo ""
}

ensure_firewall_for_current_ip() {
  if [[ -z "$azResourceGroup" || -z "$azServerName" ]]; then
    echo "WARNING: --resource-group and --server-name not provided; skipping firewall rule setup."
    return
  fi

  local ip
  ip="$(get_public_ip)"

  if [[ -z "$ip" ]]; then
    echo "WARNING: Could not detect public IP; skipping firewall rule setup."
    return
  fi

  local ruleName="${fwRulePrefix}-${ip//./-}"

  echo "Ensuring firewall rule '$ruleName' allows $ip on server '$azServerName' in RG '$azResourceGroup'..."

  if ! az postgres flexible-server firewall-rule create \
      --resource-group "$azResourceGroup" \
      --name "$azServerName" \
      --rule-name "$ruleName" \
      --start-ip-address "$ip" \
      --end-ip-address "$ip" >/dev/null 2>&1; then

    echo "Firewall rule may already exist, trying update..."
    az postgres flexible-server firewall-rule update \
      --resource-group "$azResourceGroup" \
      --name "$azServerName" \
      --rule-name "$ruleName" \
      --start-ip-address "$ip" \
      --end-ip-address "$ip" >/dev/null
  fi

  echo "Firewall rule '$ruleName' now allows IP $ip."
}

# Arg parse
while [[ $# -gt 0 ]]; do
  case "$1" in
    --help|-help|-h)
      help=true; shift
      ;;
    --pgUser|-pgUser)
      pgUser="$2"; shift 2
      ;;
    --pgHost|-pgHost)
      pgHost="$2"; shift 2
      ;;
    --pgPort|-pgPort)
      pgPort="$2"; shift 2
      ;;
    --pgSslMode|-pgSslMode)
      pgSslMode="$2"; shift 2
      ;;
    --resource-group|-g|--resourceGroup|--resource-group-name)
      azResourceGroup="$2"; shift 2
      ;;
    -s|--server-name|--serverName|-server-name|-serverName)
      azServerName="$2"; shift 2
      ;;
    --fw-rule-prefix|--fwRulePrefix)
      fwRulePrefix="$2"; shift 2
      ;;
    --team-name|-team-name|--teamName|-teamName)
      teamName="$2"; shift 2
      ;;
    --team-count|-team-count|--teamCount|-teamCount)
      teamCount="$2"; shift 2
      ;;
    *)
      echo "Unknown option: $1"
      echo "Use --help for usage."
      exit 1
      ;;
  esac
done

if $help; then
  print_help
  exit 0
fi

if [[ -z "$teamName" ]]; then
  echo "ERROR: --team-name is required."
  exit 1
fi

if ! [[ "$teamCount" =~ ^[0-9]+$ ]] || [[ "$teamCount" -lt 1 ]]; then
  echo "ERROR: --team-count must be a positive integer."
  exit 1
fi

if [[ -z "$pgHost" ]]; then
  echo "ERROR: --pgHost (or PG_HOST) is required for Azure PostgreSQL."
  exit 1
fi

if [[ -z "$pgUser" ]]; then
  echo "ERROR: --pgUser (or PG_USER) is required for Azure PostgreSQL."
  exit 1
fi

require_az_login
ensure_firewall_for_current_ip

echo "Acquiring Azure AD token for PostgreSQL..."
ACCESS_TOKEN=$(az account get-access-token \
  --resource https://ossrdbms-aad.database.windows.net \
  --query accessToken -o tsv 2>/dev/null)

if [[ -z "$ACCESS_TOKEN" ]]; then
  echo "ERROR: Failed to acquire Azure AD token. Please check 'az login'."
  exit 1
fi

echo "Token acquired. Using host=$pgHost user=$pgUser"

# Paths / logging
scriptDir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

mkdir -p "$scriptDir/logs"
logFile="$scriptDir/logs/db_setup.log"
: > "$logFile"

psql_azure() {
  local db="$1"
  shift
  PGPASSWORD="$ACCESS_TOKEN" \
    psql "host=$pgHost port=$pgPort dbname=$db user=$pgUser sslmode=$pgSslMode" "$@"
}

log_db() {
  local db="$1"
  local logfile="$2"
  local timestamp
  timestamp="$(date +"%Y-%m-%d %H:%M:%S")"
  cat >> "$logfile" <<EOF
{
  "Timestamp": "$timestamp",
  "Database": "$db",
  "Host": "$pgHost",
  "Port": "$pgPort",
  "Auth": "Azure AD token"
}
EOF
}

create_team_role() {
  local role="$1"
  echo "Ensuring role ${role} exists..."
  psql_azure "postgres" <<EOF
DO \$\$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_roles WHERE rolname = '${role}'
    ) THEN
        EXECUTE format(
            'CREATE ROLE %I NOLOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE NOINHERIT NOREPLICATION;',
            '${role}'
        );
    END IF;
END
\$\$;
EOF
}

create_team_api_role() {
  local role="$1"
  echo "Ensuring api role ${role} exists..."
  psql_azure "postgres" <<EOF
DO \$\$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_roles WHERE rolname = '${role}'
    ) THEN
        EXECUTE format(
            'CREATE ROLE %I NOLOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE NOINHERIT NOREPLICATION;',
            '${role}'
        );
    END IF;
END
\$\$;
EOF
}

# Per-team setup loop
for (( i=1; i<=teamCount; i++ )); do
  idx=$(printf "%02d" "$i")
  baseName="${teamName}${idx}"
  devDbName="${baseName}_dev"
  testDbName="${baseName}_test"

  teamRole="${baseName}"
  teamApiRole="${baseName}_apiuser"

  echo "=== Setting up roles for ${baseName}: ${teamRole}, ${teamApiRole} ==="
  create_team_role "$teamRole"
  create_team_api_role "$teamApiRole"

  # Create & init both dev and test DBs
  for dbName in "$devDbName" "$testDbName"; do
    echo "=== Setting up database ${dbName} on ${pgHost} ==="

    if [[ -f "$scriptDir/schema/initdb.sql" ]]; then
      echo "Running initdb.sql for $dbName"
      psql_azure "postgres" \
        -v "newdb=${dbName}" \
        -v "teamrole=${teamRole}" \
        -v "apiuser=${teamApiRole}" \
        -v ON_ERROR_STOP=1 \
        -f "$scriptDir/schema/initdb.sql"
    else
      echo "WARNING: schema/initdb.sql not found; skipping DB init for $dbName"
    fi

    if [[ -f "$scriptDir/schema/initschema.sql" ]]; then
      echo "Running initschema.sql on $dbName"
      psql_azure "$dbName" \
        -v "teamrole=${teamRole}" \
        -v "apiuser=${teamApiRole}" \
        -v ON_ERROR_STOP=1 \
        -f "$scriptDir/schema/initschema.sql"
    fi

    log_db "$dbName" "$logFile"
  done
done

echo "All done. Log written to $logFile"