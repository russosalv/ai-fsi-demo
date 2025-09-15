#!/bin/sh

# Configurazione dinamica dell'API URL
API_URL=${API_URL:-"https://banking-api:443/api"}

echo "Configurazione API URL: $API_URL"

# File di configurazione JSON
CONFIG_FILE="/usr/share/nginx/html/assets/config.json"

if [ -f "$CONFIG_FILE" ]; then
    echo "File di configurazione trovato, aggiornamento dell'API URL..."
    
    # Crea il nuovo file di configurazione
    cat > "$CONFIG_FILE" << EOF
{
  "apiUrl": "$API_URL",
  "production": true
}
EOF
    
    echo "Configurazione API completata: $API_URL"
else
    echo "Attenzione: File di configurazione non trovato, creazione di uno nuovo..."
    
    # Crea la directory se non esiste
    mkdir -p "/usr/share/nginx/html/assets"
    
    # Crea il file di configurazione
    cat > "$CONFIG_FILE" << EOF
{
  "apiUrl": "$API_URL",
  "production": true
}
EOF
    
    echo "File di configurazione creato: $CONFIG_FILE"
fi

# Esegui il comando originale
exec "$@"