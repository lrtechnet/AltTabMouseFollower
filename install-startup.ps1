# Install AltTabMouseFollower to Windows Startup
# Usage: .\install-startup.ps1 "C:\Path\To\AltTabMouseFollower.exe"

param(
    [Parameter(Mandatory=$true)]
    [string]$ExePath
)

# Validate the exe exists
if (-not (Test-Path $ExePath)) {
    Write-Error "Error: The file '$ExePath' does not exist."
    exit 1
}

# Get the startup folder path
$startupFolder = [System.IO.Path]::Combine(
    [Environment]::GetFolderPath('Startup')
)

$shortcutPath = Join-Path $startupFolder "AltTabMouseFollower.lnk"

# Check if shortcut already exists
if (Test-Path $shortcutPath) {
    Write-Host "Startup shortcut already exists. Updating..." -ForegroundColor Yellow
    Remove-Item $shortcutPath
}

# Create shortcut
$shell = New-Object -ComObject WScript.Shell
$shortcut = $shell.CreateShortcut($shortcutPath)
$shortcut.TargetPath = $ExePath
$shortcut.WorkingDirectory = Split-Path $ExePath
$shortcut.Description = "Alt+Tab Mouse Follower"
$shortcut.Save()

Write-Host "✓ Successfully installed to startup!" -ForegroundColor Green
Write-Host "Shortcut created at: $shortcutPath" -ForegroundColor Cyan

# Start the application immediately
Write-Host "`nStarting application..." -ForegroundColor Cyan
Start-Process $ExePath
Write-Host "✓ Application is now running!" -ForegroundColor Green

Write-Host "`nThe application will now start automatically when you log in to Windows."