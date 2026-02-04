#!/usr/bin/env bash

# This script is used to launch the web application in different environments.
# It allows the user to specify whether to run the development or production version of the application.
#
# Usage:
# ./launch-web.sh [options]
# Options:
#   --dev    Launch the development environment
#   --prod   Launch the production environment [not functional]
#   --help   Display this help message
#
# Notes:
# Ensure Node.js and npm are installed and available in your PATH.

dev=false
prod=false
help=false

# Parse arguments
for arg in "$@"; do
  case "$arg" in
    --dev)
      dev=true
      ;;
    --prod)
      prod=true
      ;;
    --help)
      help=true
      ;;
  esac
done

if [ "$help" = true ]; then
  echo "Usage: ./launch-web.sh [options]"
  echo "Options:"
  echo "  --dev    Launch the development environment"
  echo "  --prod   Launch the production environment"
  echo "  --help   Display this help message"
  exit 0
fi

if [ "$dev" = true ]; then
  npm run start:dev
elif [ "$prod" = true ]; then
  npm run start:prod
else
  npm start
fi