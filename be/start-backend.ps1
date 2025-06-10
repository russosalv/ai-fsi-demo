# Banking API Startup Script with HTTPS
Write-Host "🏦 Starting Banking API with HTTPS..." -ForegroundColor Green
Write-Host "📍 Location: Banking.API" -ForegroundColor Yellow

# Navigate to the Banking.API directory
Set-Location "Banking.API"

# Run the application with HTTPS profile
Write-Host "🚀 Running: dotnet run --launch-profile https" -ForegroundColor Cyan
Write-Host "🌐 HTTPS URL: https://localhost:7086" -ForegroundColor Green
Write-Host "📄 Swagger UI: https://localhost:7086/swagger" -ForegroundColor Green
Write-Host "⚡ HTTP URL: http://localhost:5281 (redirects to HTTPS)" -ForegroundColor Yellow
Write-Host ""
Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Red
Write-Host "----------------------------------------" -ForegroundColor Gray

dotnet run --launch-profile https 