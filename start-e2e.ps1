#!/usr/bin/env pwsh

# Script per avviare Backend e Frontend per test End-to-End
# Uso: .\start-e2e.ps1

Write-Host "Avvio Backend e Frontend per test E2E..." -ForegroundColor Green

# Funzione per pulire i processi quando lo script viene terminato
function Cleanup {
    Write-Host "`nPulizia processi in corso..." -ForegroundColor Yellow
    
    if ($backendJob) {
        Write-Host "Terminazione Backend..." -ForegroundColor Yellow
        Stop-Job $backendJob -ErrorAction SilentlyContinue
        Remove-Job $backendJob -ErrorAction SilentlyContinue
    }
    
    if ($frontendJob) {
        Write-Host "Terminazione Frontend..." -ForegroundColor Yellow
        Stop-Job $frontendJob -ErrorAction SilentlyContinue
        Remove-Job $frontendJob -ErrorAction SilentlyContinue
    }
    
    # Termina tutti i processi dotnet e ng serve
    Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Where-Object { $_.ProcessName -eq "dotnet" } | Stop-Process -Force -ErrorAction SilentlyContinue
    Get-Process -Name "node" -ErrorAction SilentlyContinue | Where-Object { $_.CommandLine -like "*ng serve*" } | Stop-Process -Force -ErrorAction SilentlyContinue
    
    Write-Host "Pulizia completata!" -ForegroundColor Green
    exit
}

# Registra il gestore per Ctrl+C
Register-EngineEvent PowerShell.Exiting -Action { Cleanup }
$null = [System.Console]::TreatControlCAsInput = $false

# Verifica che le directory esistano
if (-not (Test-Path "be/Banking.API")) {
    Write-Error "Directory Backend non trovata: be/Banking.API"
    exit 1
}

if (-not (Test-Path "fe")) {
    Write-Error "Directory Frontend non trovata: fe"
    exit 1
}

# Verifica che dotnet sia installato
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Error ".NET CLI non trovato. Installa .NET SDK"
    exit 1
}

# Verifica che npm/ng siano installati
if (-not (Get-Command npm -ErrorAction SilentlyContinue)) {
    Write-Error "npm non trovato. Installa Node.js"
    exit 1
}

Write-Host "Controllo dipendenze frontend..." -ForegroundColor Cyan
Set-Location fe
if (-not (Test-Path "node_modules")) {
    Write-Host "Installazione dipendenze npm..." -ForegroundColor Cyan
    npm install
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Errore durante l'installazione delle dipendenze npm"
        Set-Location ..
        exit 1
    }
}
Set-Location ..

try {
    
    Write-Host "Avvio Backend (.NET API)..." -ForegroundColor Cyan
    
    # Avvia il backend in background
    $backendJob = Start-Job -ScriptBlock {
        Set-Location $using:PWD
        Set-Location be/Banking.API
        $env:ASPNETCORE_ENVIRONMENT = "Development"
        dotnet run --launch-profile https
    }
    
    # Aspetta un po' per il backend
    Write-Host "Attesa avvio Backend..." -ForegroundColor Yellow
    Start-Sleep -Seconds 10
    
    Write-Host "Avvio Frontend (Angular)..." -ForegroundColor Cyan
    
    # Avvia il frontend in background
    $frontendJob = Start-Job -ScriptBlock {
        Set-Location $using:PWD
        Set-Location fe
        $env:BACKEND_API_URL = "https://localhost:7086/api"
        npx ng serve --port 4200 --host 0.0.0.0
    }
    
    Write-Host "Attesa avvio Frontend..." -ForegroundColor Yellow
    Start-Sleep -Seconds 15
    
    Write-Host "Servizi avviati!" -ForegroundColor Green
    Write-Host "Backend: https://localhost:7086" -ForegroundColor Cyan
    Write-Host "Frontend: http://localhost:4200" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Stato servizi:" -ForegroundColor White
    Write-Host "   Backend Job ID: $($backendJob.Id)" -ForegroundColor Gray
    Write-Host "   Frontend Job ID: $($frontendJob.Id)" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Ora puoi eseguire i tuoi test E2E!" -ForegroundColor Green
    Write-Host "Premi Ctrl+C per terminare entrambi i servizi" -ForegroundColor Yellow
    Write-Host ""
    
    # Mantieni lo script in esecuzione e mostra i log
    while ($true) {
        # Controlla se i job sono ancora in esecuzione
        if ($backendJob.State -eq "Failed") {
            Write-Host "Backend e' terminato con errore!" -ForegroundColor Red
            Receive-Job $backendJob
            break
        }
        
        if ($frontendJob.State -eq "Failed") {
            Write-Host "Frontend e' terminato con errore!" -ForegroundColor Red
            Receive-Job $frontendJob
            break
        }
        
        Start-Sleep -Seconds 5
    }
    
} catch {
    Write-Error "Errore durante l'avvio: $_"
} finally {
    Cleanup
} 