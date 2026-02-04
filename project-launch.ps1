# This script runs the launch scripts for both the Front and Back End of the project together.
# It sets the working directory to the appropriate locations and starts the respective processes.
# Ensure that the necessary prerequisites are installed and available in your PATH.
#
# Usage: .\project-launch.ps1 [options]
# Note: Runs launch scripts for Front and Back End with default parameters.
# Options:
# -help: Display this help message

param (
    [switch]$help
)

if ($help) {
    Write-Host "Usage: .\project-launch.ps1 [options]"
    Write-Host "Note: Runs launch scripts for Front and Back End with default parameters."
    Write-Host "Options:"
    Write-Host "-help: Display this help message"
    exit
}

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

Set-Location $scriptDir\CAM-CMS-Server

Start-Process PowerShell .\launch-server.ps1

Set-Location $scriptDir\CAM-CMS

Start-Process PowerShell .\launch-web.ps1