#!/usr/bin/env bash
set -euo pipefail

help=false

pgUser=""
pgHost=""          # required for Azure (server FQDN)
pgPort="5432"

dbName=""
dbRole=""

# Optional: if you need a specific tenant/subscription context
azureTenantId="${AZURE_TENANT_ID:-}"
azureSubscriptionId="${AZURE_SUBSCRIPTION_ID:-}"

# Azure PostgreSQL Entra token resource (recommended by Azure docs/CLI)
AZURE_PG_TOKEN_RESOURCE_TYPE="oss-rdbms"

# Enforce TLS for Azure Postgres
sslMode="require"

# Migration history config
MIGRATION_TABLE="db_migration"
MIGRATION_ID_COLUMN="migration_id"

print_help() {
  cat <<'EOF'
Usage: ./db-run-migration-azure.sh [options]

Runs all SQL migrations in ./migration/*.sql against an Azure Database for PostgreSQL
(flexible server) using Microsoft Entra ID authentication (token-based), skipping
those already recorded in the db_migration table.

Options:
  --help                   Display this help message
  --pgUser <user>          PostgreSQL Entra principal to connect as
                           (ex: user@domain.com, group name, or service principal name)
  --pgHost <host>          Azure PostgreSQL server host (FQDN)
                           ex: myserver.postgres.database.azure.com
  --pgPort <port>          PostgreSQL port (default: 5432)
  --db-name <name>         Database name to target (ex: team01_dev)
  --role-name <role>       OPTIONAL: role to SET ROLE to before each migration
                           ex: team01
  --sslmode <mode>         SSL mode (default: require)

Entra auth behavior:
- Requires Azure CLI login (az login).
- Retrieves an Entra access token via:
    az account get-access-token --resource-type oss-rdbms
  and passes the token to psql via PGPASSWORD.
- Refreshes token before each psql call (tokens are short-lived).

Examples:
  ./db-run-migration-azure.sh \
    --pgUser team01_apiuser@yourtenant.com \
    --pgHost yourserver.postgres.database.azure.com \
    --db-name team01_dev \
    --role-name team01

  # If your Azure CLI context needs a specific tenant/subscription:
  AZURE_TENANT_ID="xxxx-..." AZURE_SUBSCRIPTION_ID="yyyy-..." \
    ./db-run-migration-azure.sh --pgUser ... --pgHost ... --db-name ...
EOF
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
    --db-name|--dbName|-db-name|-dbName)
      dbName="$2"; shift 2
      ;;
    --role-name|--role|-role-name|-role)
      dbRole="$2"; shift 2
      ;;
    --sslmode|-sslmode)
      sslMode="$2"; shift 2
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

# Validation
if [[ -z "$dbName" ]]; then
  echo "ERROR: --db-name is required."
  exit 1
fi

if [[ -z "$pgUser" ]]; then
  echo "ERROR: --pgUser is required."
  exit 1
fi

if [[ -z "$pgHost" ]]; then
  echo "ERROR: --pgHost is required for Azure (use the server FQDN)."
  exit 1
fi

# Pre-flight checks
if ! command -v psql >/dev/null 2>&1; then
  echo "ERROR: psql not found. Install PostgreSQL client tools."
  exit 1
fi

if ! command -v az >/dev/null 2>&1; then
  echo "ERROR: az (Azure CLI) not found. Install Azure CLI and run 'az login'."
  exit 1
fi

# Paths
scriptDir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

echo "Target database: ${dbName}"
echo "Host: ${pgHost}, Port: ${pgPort}, User: ${pgUser}"
echo "SSL mode: ${sslMode}"
if [[ -n "$dbRole" ]]; then
  echo "SET ROLE: ${dbRole}"
fi
echo

# Ensure migration directory exists and has files
migDir="$scriptDir/migration"
if [[ ! -d "$migDir" ]]; then
  echo "ERROR: Migration directory '$migDir' does not exist."
  exit 1
fi

shopt -s nullglob
migrations=("$migDir"/*.sql)
shopt -u nullglob

if (( ${#migrations[@]} == 0 )); then
  echo "No migration files found in '$migDir'. Nothing to do."
  exit 0
fi

echo "Found ${#migrations[@]} migration(s):"
for mig in "${migrations[@]}"; do
  echo "  - $(basename "$mig")"
done
echo

# Acquire a fresh Entra token and export it as PGPASSWORD for psql to use.
get_and_export_pg_token() {
  # Optionally pin tenant/subscription context if provided via env vars
  if [[ -n "$azureTenantId" ]]; then
    az account set --tenant "$azureTenantId" >/dev/null
  fi
  if [[ -n "$azureSubscriptionId" ]]; then
    az account set --subscription "$azureSubscriptionId" >/dev/null
  fi

  # Token for Azure Database for PostgreSQL
  # (psql is not Entra-aware, so token goes in the password field)
  local token
  token="$(
    az account get-access-token \
      --resource-type "$AZURE_PG_TOKEN_RESOURCE_TYPE" \
      --query accessToken -o tsv
  )"

  if [[ -z "$token" ]]; then
    echo "ERROR: Failed to acquire Entra access token. Ensure 'az login' succeeded."
    exit 1
  fi

  export PGPASSWORD="$token"
}

# Wrapper to ensure token freshness for every call
psql_azure() {
  get_and_export_pg_token
  PGSSLMODE="$sslMode" psql "$@"
}

has_migration_table=$(
  psql_azure \
    -h "$pgHost" -p "$pgPort" \
    -U "$pgUser" -d "$dbName" \
    -tA -c "SELECT to_regclass('${MIGRATION_TABLE}') IS NOT NULL;" | xargs
)

applied_migrations=""

if [[ "$has_migration_table" == "t" ]]; then
  echo "Migration history table '${MIGRATION_TABLE}' found. Loading applied migrations..."
  applied_migrations=$(
    psql_azure \
      -h "$pgHost" -p "$pgPort" \
      -U "$pgUser" -d "$dbName" \
      -tA -c "SELECT ${MIGRATION_ID_COLUMN} FROM ${MIGRATION_TABLE};"
  )
  echo "Already applied:"
  echo "$applied_migrations"
  echo
else
  echo "Migration history table '${MIGRATION_TABLE}' does not exist yet."
  echo "Assuming this is the first run; all migrations will be applied."
  echo
fi

# Run migrations in order
for mig in "${migrations[@]}"; do
  base=$(basename "$mig")
  mig_id="${base%.sql}"

  if [[ -n "$applied_migrations" ]] && echo "$applied_migrations" | grep -Fxq "$mig_id"; then
    echo "Skipping already-applied migration: $base"
    continue
  fi

  echo "Running migration: $base"

  if [[ -n "$dbRole" ]]; then
    {
      echo "SET ROLE \"${dbRole}\";"
      cat "$mig"
    } | psql_azure \
          -h "$pgHost" -p "$pgPort" \
          -U "$pgUser" \
          -d "$dbName" \
          -v ON_ERROR_STOP=1
  else
    psql_azure \
      -h "$pgHost" -p "$pgPort" \
      -U "$pgUser" \
      -d "$dbName" \
      -v ON_ERROR_STOP=1 \
      -f "$mig"
  fi

  echo "Done: $base"
  echo
done

echo "All applicable migrations applied successfully to ${dbName}."