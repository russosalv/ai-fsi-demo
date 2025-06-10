# Test script for both PowerShell and Bash versions
# This file can be used to verify the functionality

Write-Host "Testing PowerShell version..." -ForegroundColor Green
& ".\generate-valid-codes.ps1"

Write-Host "`n" + "="*50 -ForegroundColor Blue
Write-Host "To test the Bash version, run in Git Bash or WSL:" -ForegroundColor Yellow
Write-Host "./generate-valid-codes.sh" -ForegroundColor Cyan
