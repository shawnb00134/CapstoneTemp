#!/usr/bin/env bash
set -euo pipefail

help=false
pgUser="postgres"
pgHost="localhost"
pgPwd=""
pgPort="5432"

dbName=""

print_help() {
  cat <<'EOF'
Usage: ./db-local-setup.sh [options]

Creates a single database and roles for local use.

Options:
  --help                   Display this help message
  --pgUser <user>          PostgreSQL superuser / admin (default: postgres)
  --pgHost <host>          PostgreSQL host (default: localhost)
  --pgPwd <pwd>            PostgreSQL admin password
  --pgPort <port>          PostgreSQL port (default: 5432)
  --name <db>              Name of the database to create
                           e.g. "camcms" -> DB camcms, roles camcms and camcms_apiuser

Behavior:
- Creates:
    DB:   <name>
    Role: <name>           (group/owner role, NOLOGIN)
    Role: <name>_apiuser   (app/API role, NOLOGIN here)
- Runs:
    schema/initdb.sql     on postgres (to create DB and grant roles)
    schema/initschema.sql on <name> (to create schema objects / permissions)

Prereqs:
- psql in PATH.
- schema/initdb.sql and schema/initschema.sql present next to this script in ./schema.
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
    --name|--db-name|-name|-db-name)
      dbName="$2"; shift 2
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

if [[ -z "$dbName" ]]; then
  echo "ERROR: --name is required."
  exit 1
fi

# Paths / logging
scriptDir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

mkdir -p "$scriptDir/logs"
logFile="$scriptDir/logs/db_setup_local_single.log"
: > "$logFile"

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

create_role_nologin() {
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

groupRole="${dbName}"
apiRole="${dbName}_apiuser"

echo "=== Creating local environment ==="
echo "Admin user: ${pgUser}@${pgHost}:${pgPort}"
echo "Database:   ${dbName}"
echo "Roles:      ${groupRole}, ${apiRole}"
echo

echo "Ensuring roles exist..."
create_role_nologin "$groupRole"
create_role_nologin "$apiRole"

echo "=== Setting up database ${dbName} ==="

if [[ -f "$scriptDir/schema/initdb.sql" ]]; then
  echo "Running initdb.sql for ${dbName}"
  PGPASSWORD="$pgPwd" psql \
    -h "$pgHost" -p "$pgPort" \
    -U "$pgUser" \
    -d "postgres" \
    -v "newdb=${dbName}" \
    -v "teamrole=${groupRole}" \
    -v "apiuser=${apiRole}" \
    -v ON_ERROR_STOP=1 \
    -f "$scriptDir/schema/initdb.sql"
else
  echo "WARNING: schema/initdb.sql not found; skipping DB init for ${dbName}"
fi

if [[ -f "$scriptDir/schema/initschema.sql" ]]; then
  echo "Running initschema.sql on ${dbName}"
  PGPASSWORD="$pgPwd" psql \
    -h "$pgHost" -p "$pgPort" \
    -U "$pgUser" \
    -d "$dbName" \
    -v "teamrole=${groupRole}" \
    -v "apiuser=${apiRole}" \
    -v ON_ERROR_STOP=1 \
    -f "$scriptDir/schema/initschema.sql"
fi

log_db "$dbName" "$logFile"

echo "All done. Personal local DB '${dbName}' created."
echo "Log written to $logFile"