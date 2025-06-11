#!/usr/bin/env pwsh
param(
    [switch]$Build,
    [switch]$Detached,
    [switch]$Down,
    [switch]$Logs,
    [string]$Service = ""
)

Write-Host "Banking Application - Docker Manager" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green

if ($Down) {
    Write-Host "Fermando tutti i servizi..." -ForegroundColor Yellow
    docker-compose down -v
    Write-Host "Servizi fermati!" -ForegroundColor Green
    exit 0
}

if ($Logs) {
    if ($Service) {
        Write-Host "Visualizzando i log del servizio: $Service" -ForegroundColor Yellow
        docker-compose logs -f $Service
    } else {
        Write-Host "Visualizzando i log di tutti i servizi..." -ForegroundColor Yellow
        docker-compose logs -f
    }
    exit 0
}

$buildFlag = if ($Build) { "--build" } else { "" }
$detachedFlag = if ($Detached) { "-d" } else { "" }
$serviceParam = if ($Service) { $Service } else { "" }

if ($Service) {
    Write-Host "Avviando il servizio: $Service" -ForegroundColor Yellow
} else {
    Write-Host "Avviando tutti i servizi..." -ForegroundColor Yellow
}

Write-Host "Comando: docker-compose up $detachedFlag $buildFlag $serviceParam" -ForegroundColor Cyan

try {
    if ($serviceParam) {
        docker-compose up $detachedFlag $buildFlag $serviceParam
    } else {
        docker-compose up $detachedFlag $buildFlag
    }
    
    if ($?) {
        Write-Host "" -ForegroundColor Green
        Write-Host "✅ Applicazione avviata con successo!" -ForegroundColor Green
        Write-Host "" -ForegroundColor Green
        Write-Host "Accesso ai servizi:" -ForegroundColor White
        Write-Host "  • Frontend:    http://localhost:4200" -ForegroundColor Cyan
        Write-Host "  • Backend API: https://localhost:7086/api" -ForegroundColor Cyan
        Write-Host "  • Swagger:     https://localhost:7086/swagger" -ForegroundColor Cyan
        Write-Host "" -ForegroundColor Green
        Write-Host "Per visualizzare i log: $PSCommandPath -Logs" -ForegroundColor Gray
        Write-Host "Per fermare i servizi: $PSCommandPath -Down" -ForegroundColor Gray
    } else {
        Write-Host "❌ Errore nell'avvio dell'applicazione!" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "❌ Errore: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Esempio di utilizzo:
<#
# Avvio completo con build
.\start-docker.ps1 -Build

# Avvio in background
.\start-docker.ps1 -Detached -Build

# Avvio solo backend
.\start-docker.ps1 -Service banking-api -Build

# Visualizza log
.\start-docker.ps1 -Logs

# Visualizza log di un servizio specifico
.\start-docker.ps1 -Logs -Service banking-api

# Ferma tutto
.\start-docker.ps1 -Down
#> 