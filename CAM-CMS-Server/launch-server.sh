#!/usr/bin/env bash
set -euo pipefail

# launch-server.sh
# Runs the ASP.NET Core backend locally:
#   default -> LocalPostgreSqlDataContext (ASPNETCORE_ENVIRONMENT unset)
#   --dev   -> DevPostgreSqlDataContext   (ASPNETCORE_ENVIRONMENT=dev)
#   --test  -> TestPostgreSqlDataContext  (ASPNETCORE_ENVIRONMENT=test)

MODE="local"
PROJECT_DIR="./CAM-CMS-Server"
URLS="https://localhost:7079;http://localhost:5142"

print_help() {
  cat <<'EOF'
Usage: ./launch-server.sh [--dev | --test] [--urls "<https;http>"] [-- <dotnet args>]

Options:
  --dev                 Use DevPostgreSqlDataContext (ASPNETCORE_ENVIRONMENT=dev)
  --test                Use TestPostgreSqlDataContext (ASPNETCORE_ENVIRONMENT=test)
  --urls "<...>"        Override ASPNETCORE_URLS (default: https://localhost:7079;http://localhost:5142)
  --help                Show this help
  --                    Pass remaining args to dotnet run

Examples:
  ./launch-server.sh
  ./launch-server.sh --dev
  ./launch-server.sh --test
  ./launch-server.sh --dev -- --no-launch-profile
EOF
}

DOTNET_ARGS=()

while [[ $# -gt 0 ]]; do
  case "$1" in
    --dev)
      MODE="dev"
      shift
      ;;
    --test)
      MODE="test"
      shift
      ;;
    --urls)
      URLS="${2:-}"
      if [[ -z "$URLS" ]]; then
        echo "Error: --urls requires a value" >&2
        exit 2
      fi
      shift 2
      ;;
    --help|-h)
      print_help
      exit 0
      ;;
    --)
      shift
      DOTNET_ARGS+=("$@")
      break
      ;;
    *)
      echo "Unknown option: $1" >&2
      echo "Run with --help for usage." >&2
      exit 2
      ;;
  esac
done

export ASPNETCORE_URLS="$URLS"

case "$MODE" in
  dev)
    export ASPNETCORE_ENVIRONMENT="dev"
    ;;
  test)
    export ASPNETCORE_ENVIRONMENT="test"
    ;;
  local)
    unset ASPNETCORE_ENVIRONMENT || true
    ;;
esac

echo "Running backend with MODE=$MODE"
echo "ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT-<unset>}"
echo "ASPNETCORE_URLS=$ASPNETCORE_URLS"
echo

cd "$PROJECT_DIR"
dotnet run --no-launch-profile "${DOTNET_ARGS[@]}"