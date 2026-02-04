#!/usr/bin/env bash
set -euo pipefail

help=false

pgUser=""
pgHost="localhost"
pgPort="5432"
pgPwd="${PGPASSWORD:-}"

dbName=""
dbRole=""

# Migration history config
MIGRATION_TABLE="db_migration"
MIGRATION_ID_COLUMN="migration_id"

print_help() {
  cat <<'EOF'
Usage: ./db-run-migrations.sh [options]

Runs all SQL migrations in ./migration/*.sql against a specific database,
skipping those already recorded in the db_migration table.

Options:
  --help                   Display this help message
  --pgUser <user>          PostgreSQL user (login / app user)
  --pgHost <host>          PostgreSQL host (default: localhost)
  --pgPort <port>          PostgreSQL port (default: 5432)
  --pgPwd  <pwd>           PostgreSQL password (or set PGPASSWORD env)
  --db-name <name>         FULL database name to target
                           e.g. team01_dev, team01_test, team01_local
  --role-name <role>       OPTIONAL: role to SET ROLE to before each migration
                           e.g. team01

Behavior:
- Connects to the given --db-name as --pgUser.
- If --role-name is provided, runs:   SET ROLE <role>;
  before executing each migration file.
- Checks for a migration history table (db_migration by default).
  - If it does not exist, runs all migrations.
  - If it exists, skips any migration whose ID is already recorded there.
- Migration IDs are derived from the filename without .sql
  (e.g. 0001.sql -> 0001) and compared to the column MIGRATION_ID_COLUMN.

Examples:
  # Run migrations on team01_dev, setting group role team01
  PGPASSWORD='studentpw' ./db-run-migrations.sh \
    --pgUser team01_apiuser \
    --pgHost localhost \
    --db-name team01_dev \
    --role-name team01

  # Run migrations on a custom local DB without SET ROLE
  PGPASSWORD='studentpw' ./db-run-migrations.sh \
    --pgUser student_user \
    --pgHost localhost \
    --db-name my_local_db
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
    --pgPwd|-pgPwd)
      pgPwd="$2"; shift 2
      ;;
    --db-name|--dbName|-db-name|-dbName)
      dbName="$2"; shift 2
      ;;
    --role-name|--role|-role-name|-role)
      dbRole="$2"; shift 2
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

if [[ -z "$pgPwd" ]]; then
  echo "WARNING: No password provided (pgPwd/PGPASSWORD)."
  echo "         psql will prompt interactively if required."
else
  export PGPASSWORD="$pgPwd"
fi

# Paths
scriptDir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

echo "Target database: ${dbName}"
echo "Host: ${pgHost}, Port: ${pgPort}, User: ${pgUser}"
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

has_migration_table=$(
  psql \
    -h "$pgHost" -p "$pgPort" \
    -U "$pgUser" -d "$dbName" \
    -tA -c "SELECT to_regclass('${MIGRATION_TABLE}') IS NOT NULL;" | xargs
)

applied_migrations=""

if [[ "$has_migration_table" == "t" ]]; then
  echo "Migration history table '${MIGRATION_TABLE}' found. Loading applied migrations..."
  applied_migrations=$(
    psql \
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
    } | psql \
          -h "$pgHost" -p "$pgPort" \
          -U "$pgUser" \
          -d "$dbName" \
          -v ON_ERROR_STOP=1
  else
    psql \
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