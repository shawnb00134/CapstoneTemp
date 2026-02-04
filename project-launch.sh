#!/usr/bin/env bash

# This script runs the launch scripts for both the Front and Back End of the project together.
# It sets the working directory to the appropriate locations and starts the respective processes.
# Ensure that the necessary prerequisites are installed and available in your PATH.
#
# Usage: ./project-launch.sh [options]
# Note: Runs launch scripts for Front and Back End with default parameters.
# Options:
#   --dev    Run both Front and Back End in dev mode
#   --test   Run both Front and Back End in test mode
#   --help   Display this help message

dev=false
test=false
help=false

# Parse arguments
for arg in "$@"; do
  case "$arg" in
    --dev)
      dev=true
      ;;
    --test)
      test=true
      ;;
    --help)
      help=true
      ;;
  esac
done

if [ "$help" = true ]; then
  echo "Usage: ./project-launch.sh [options]"
  echo "Note: Runs launch scripts for Front and Back End with default parameters."
  echo "Options:"
  echo "  --dev    Run both Front and Back End in dev mode"
  echo "  --test   Run both Front and Back End in test mode"
  echo "  --help   Display this help message"
  exit 0
fi

scriptDir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

cd "$scriptDir/CAM-CMS-Server" || exit 1

if [ "$dev" = true ]; then
  ./launch-server.sh --dev &
elif [ "$test" = true ]; then
  ./launch-server.sh --test &
else
  ./launch-server.sh &
fi

cd "$scriptDir/CAM-CMS" || exit 1

./launch-web.sh &

wait
