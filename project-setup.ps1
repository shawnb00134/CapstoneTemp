# Batch file to create a new database and adjust the connection string in the appsettings.json file
# Additionally, it runs npm install to setup web dependencies.
# Usage: .\project-setup.ps1 [options]
# Options:
# "-full": Run the full setup including database creation
# "-help": Display this help message
# "-pgUser": PostgreSQL user (default: postgres)
# "-pgHost": PostgreSQL host (default: localhost)
# "-pgDb": PostgreSQL database name (default: cam_cms)
# "-pgPwd": PostgreSQL password (default: Password123)
# "-pgPort": PostgreSQL port (default: 5432)
# "-appOwnerPwd": Database admin password (default: Password123!)
# Notes:
# Ensure PostgreSQL is installed and the psql command is available in your PATH.

param (
    [switch]$full,
    [switch]$help,
    [string]$pgUser = "postgres",
    [string]$pgHost = "localhost",
    [string]$pgDb = "cam_cms",
    [string]$pgPwd = "Password123", # Default password for demonstration purposes
    [string]$pgPort = "5432",
    [string]$appOwnerPwd = "Password123!" # Default password for demonstration purposes
)

if ($help) {
    Write-Host "Usage: .\project-setup.ps1 [options]"
    Write-Host "Options:"
    Write-Host "-full: Run the full setup including database creation"
    Write-Host "-help: Display this help message"
    Write-Host "-pgUser: PostgreSQL user (default: postgres)"
    Write-Host "-pgHost: PostgreSQL host (default: localhost)"
    Write-Host "-pgDb: PostgreSQL database name (default: cam_cms)"
    Write-Host "-pgPwd: PostgreSQL password (default: Password123)"
    Write-Host "-pgPort: PostgreSQL port (default: 5432)"
    Write-Host "-appOwnerPwd: Database admin password (default: Password123!)"
    exit
}

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

if ($full) {
    # Run database setup script
    $env:PGPASSWORD = $pgPwd
    & "$scriptDir\db\db-setup.ps1" -pgUser $pgUser -pgHost $pgHost -pgDb $pgDb -pgPwd $pgPwd -pgPort $pgPort -appOwnerPwd $appOwnerPwd
}

# Update appsettings.json with the new database connection string
$appSettingsPath = "$scriptDir\CAM-CMS-Server\CAM-CMS-Server\appsettings.json"
$dbLogFile = "$scriptDir\db\logs\db_setup.log"
$json = Get-Content -Path $dbLogFile -Raw | ConvertFrom-Json
$pgHost = $json.Host
$pgPort = $json.Port
$pgDb = $json.Database
$appOwnerPwd = $json.AppOwnerPassword
$connectionString = "Host=$pgHost;Port=$pgPort;Database=$pgDb;User Id=appowner;Include Error Detail=true;Pooling=false;Enlist=false;Connection Idle Lifetime=5;Maximum Pool Size=20;Password=$appOwnerPwd"
$json = Get-Content -Path $appSettingsPath -Raw | ConvertFrom-Json
$json.ConnectionStrings.WebApiPostgreSQLDevDatabase = $connectionString

$json | ConvertTo-Json -Depth 10 | Set-Content -Path $appSettingsPath

# Run npm install to set up web dependencies
$npmInstallPath = "$scriptDir\CAM-CMS"
Write-Host "Running npm install in $npmInstallPath..."
Set-Location -Path $npmInstallPath

npm install