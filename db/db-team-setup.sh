#!/usr/bin/env bash
set -euo pipefail

help=false
pgUser="postgres"
pgHost="localhost"
pgPwd=""
pgPort="5432"

teamName=""       # e.g. "team"
teamCount=0       # e.g. 2 -> team01, team02

print_help() {
  cat <<'EOF'
Usage: ./db-team-setup.sh [options]

Options:
  --help                   Display this help message
  --pgUser <user>          PostgreSQL superuser / admin (default: postgres)
  --pgHost <host>          PostgreSQL host (default: localhost)
  --pgPwd <pwd>            PostgreSQL admin password
  --pgPort <port>          PostgreSQL port (default: 5432)
  --team-name <base>       Base name for per-team DBs/roles (e.g. "team")
  --team-count <n>         How many teams to create (e.g. 2 -> team01, team02)

Behavior:
- For each i in [1..team-count], we create:
    DBs:  <team-name><NN>_dev   and   <team-name><NN>_test
          (NN = zero-padded index, e.g. 01)
    Role: <team-name><NN>          (team-level group role, NOLOGIN)
    Role: <team-name><NN>_apiuser  (per-team role)
- schema/initdb.sql and schema/initschema.sql are run as --pgUser
  inside each team database.

Prereqs: psql in PATH.
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
    --pgPwd|-pgPwd)
      pgPwd="$2"; shift 2
      ;;
    --pgPort|-pgPort)
      pgPort="$2"; shift 2
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

# Paths / logging
scriptDir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

mkdir -p "$scriptDir/logs"
logFile="$scriptDir/logs/db_setup.log"
: > "$logFile"

# Helpers

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
  "Port": "$pgPort"
}
EOF
}

create_team_role() {
  local role="$1"
  PGPASSWORD="$pgPwd" psql -h "$pgHost" -p "$pgPort" -U "$pgUser" <<EOF
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
  PGPASSWORD="$pgPwd" psql -h "$pgHost" -p "$pgPort" -U "$pgUser" <<EOF
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

  # Create and init both dev and test databases
  for dbName in "$devDbName" "$testDbName"; do
    echo "=== Setting up database ${dbName} ==="

    if [[ -f "$scriptDir/schema/initdb.sql" ]]; then
      echo "Running initdb.sql for $dbName"
      PGPASSWORD="$pgPwd" psql \
        -h "$pgHost" -p "$pgPort" \
        -U "$pgUser" \
        -d "postgres" \
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
      PGPASSWORD="$pgPwd" psql \
        -h "$pgHost" -p "$pgPort" \
        -U "$pgUser" \
        -d "$dbName" \
        -v "teamrole=${teamRole}" \
        -v "apiuser=${teamApiRole}" \
        -v ON_ERROR_STOP=1 \
        -f "$scriptDir/schema/initschema.sql"
    fi

    log_db "$dbName" "$logFile"
  done
done

echo "All done. Log written to $logFile"