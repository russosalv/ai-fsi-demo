param(
    [Parameter(Mandatory=$false)]
    [string]$ApiUrl = "https://localhost:7086/api"
)

Write-Host "Configurazione API URL: $ApiUrl" -ForegroundColor Green

# Percorso del file di configurazione
$configPath = "src/app/config/app.config.ts"

if (Test-Path $configPath) {
    # Leggi il contenuto del file
    $content = Get-Content $configPath -Raw
    
    # Sostituisci l'URL dell'API
    $newContent = $content -replace "apiUrl: '[^']*'", "apiUrl: '$ApiUrl'"
    
    # Scrivi il file aggiornato
    Set-Content $configPath $newContent -Encoding UTF8
    
    Write-Host "Configurazione aggiornata con successo!" -ForegroundColor Green
    Write-Host "API URL impostato su: $ApiUrl" -ForegroundColor Yellow
} else {
    Write-Host "Errore: File di configurazione non trovato in $configPath" -ForegroundColor Red
    exit 1
} 