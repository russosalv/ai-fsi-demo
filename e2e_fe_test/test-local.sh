#!/bin/bash

# Script per esecuzione locale dei test E2E Banking Portal
# Uso: ./test-local.sh [--headless] [--install]

set -e  # Exit on any error

# Colori per output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Funzione di logging colorato
log() {
    echo -e "${BLUE}[$(date +'%Y-%m-%d %H:%M:%S')]${NC} $1"
}

error() {
    echo -e "${RED}[ERROR]${NC} $1" >&2
}

success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

# Parsing argomenti
HEADLESS=false
INSTALL_DEPS=false

for arg in "$@"; do
    case $arg in
        --headless)
            HEADLESS=true
            shift
            ;;
        --install)
            INSTALL_DEPS=true
            shift
            ;;
        --help|-h)
            echo "Uso: $0 [OPZIONI]"
            echo ""
            echo "OPZIONI:"
            echo "  --headless    Esegue il test in modalità headless (senza interfaccia grafica)"
            echo "  --install     Installa le dipendenze prima di eseguire il test"
            echo "  --help, -h    Mostra questo messaggio di aiuto"
            echo ""
            echo "ESEMPI:"
            echo "  $0                    # Esegue il test con interfaccia grafica"
            echo "  $0 --headless        # Esegue il test in modalità headless"
            echo "  $0 --install          # Installa dipendenze ed esegue il test"
            echo "  $0 --install --headless  # Installa dipendenze ed esegue il test headless"
            exit 0
            ;;
        *)
            error "Argomento sconosciuto: $arg"
            echo "Usa --help per vedere le opzioni disponibili"
            exit 1
            ;;
    esac
done

# Intestazione
echo ""
log "🚀 Test E2E Banking Portal - Esecuzione Locale"
echo ""

# Verifica che siamo nella cartella corretta
if [ ! -f "banking-portal-e2e.js" ]; then
    error "File banking-portal-e2e.js non trovato!"
    error "Assicurati di essere nella cartella e2e_fe_test"
    exit 1
fi

# Installazione dipendenze se richiesto
if [ "$INSTALL_DEPS" = true ]; then
    log "📦 Installazione dipendenze..."
    if command -v npm >/dev/null 2>&1; then
        npm install
        success "Dipendenze installate con successo"
    else
        error "npm non trovato! Installa Node.js prima di continuare"
        exit 1
    fi
    echo ""
fi

# Verifica presenza node_modules
if [ ! -d "node_modules" ]; then
    warning "Cartella node_modules non trovata"
    log "Installazione automatica delle dipendenze..."
    npm install
    echo ""
fi

# Verifica che i servizi siano in esecuzione
log "🔍 Verifica servizi in esecuzione..."

# Controllo frontend
if curl -f http://localhost:4200 >/dev/null 2>&1; then
    success "✅ Frontend attivo su http://localhost:4200"
else
    error "❌ Frontend non raggiungibile su http://localhost:4200"
    warning "Assicurati che il frontend sia avviato prima di eseguire i test"
    exit 1
fi

# Controllo backend (prova diverse porte comuni)
BACKEND_FOUND=false
for port in 5000 5001 3000 8080; do
    if curl -f http://localhost:$port >/dev/null 2>&1; then
        success "✅ Backend attivo su http://localhost:$port"
        BACKEND_FOUND=true
        break
    fi
done

if [ "$BACKEND_FOUND" = false ]; then
    warning "⚠️  Backend non rilevato automaticamente"
    warning "Il test potrebbe fallire se il backend non è in esecuzione"
    echo ""
fi

# Esecuzione test
log "🧪 Avvio test E2E..."
echo ""

# Determina il comando da eseguire
if [ "$HEADLESS" = true ]; then
    log "Modalità: Headless (senza interfaccia grafica)"
    COMMAND="npm run test:headless"
else
    log "Modalità: Con interfaccia grafica"
    COMMAND="npm test"
fi

echo ""
log "Esecuzione comando: $COMMAND"
echo ""

# Esecuzione del test con gestione dell'exit code
if eval $COMMAND; then
    TEST_EXIT_CODE=$?
    echo ""
    if [ $TEST_EXIT_CODE -eq 1 ]; then
        success "🎉 TEST E2E COMPLETATO CON SUCCESSO!"
        success "Tutti gli step sono stati eseguiti correttamente"
    else
        warning "Test completato con exit code $TEST_EXIT_CODE"
    fi
else
    TEST_EXIT_CODE=$?
    echo ""
    error "❌ TEST E2E FALLITO!"
    error "Exit code: $TEST_EXIT_CODE"
    
    # Cerca screenshot di errore
    if ls error-*.png >/dev/null 2>&1; then
        warning "Screenshot di errore disponibili:"
        ls -la error-*.png
    fi
    
    exit $TEST_EXIT_CODE
fi

echo ""
log "✨ Esecuzione completata" 