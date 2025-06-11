#!/bin/bash

# Banking Application - Docker Manager
echo -e "\033[32mBanking Application - Docker Manager\033[0m"
echo -e "\033[32m=====================================\033[0m"

# Funzione per mostrare l'help
show_help() {
    echo "Uso: $0 [opzioni]"
    echo ""
    echo "Opzioni:"
    echo "  -b, --build      Forza il rebuild delle immagini"
    echo "  -d, --detached   Avvia in modalità detached (background)"
    echo "  -s, --service    Avvia solo un servizio specifico (banking-api o banking-frontend)"
    echo "  -l, --logs       Visualizza i log (opzionale: specifica il servizio)"
    echo "  --down           Ferma tutti i servizi"
    echo "  -h, --help       Mostra questo messaggio di aiuto"
    echo ""
    echo "Esempi:"
    echo "  $0 -b              # Avvio completo con build"
    echo "  $0 -d -b           # Avvio in background con build"
    echo "  $0 -s banking-api  # Avvio solo backend"
    echo "  $0 -l              # Visualizza log di tutti i servizi"
    echo "  $0 -l banking-api  # Visualizza log del backend"
    echo "  $0 --down          # Ferma tutti i servizi"
}

# Variabili default
BUILD=""
DETACHED=""
SERVICE=""
LOGS=""
DOWN=""

# Parsing degli argomenti
while [[ $# -gt 0 ]]; do
    case $1 in
        -b|--build)
            BUILD="--build"
            shift
            ;;
        -d|--detached)
            DETACHED="-d"
            shift
            ;;
        -s|--service)
            SERVICE="$2"
            shift 2
            ;;
        -l|--logs)
            LOGS="true"
            if [[ $2 && $2 != -* ]]; then
                SERVICE="$2"
                shift 2
            else
                shift
            fi
            ;;
        --down)
            DOWN="true"
            shift
            ;;
        -h|--help)
            show_help
            exit 0
            ;;
        *)
            echo -e "\033[31mOpzione sconosciuta: $1\033[0m"
            show_help
            exit 1
            ;;
    esac
done

# Ferma i servizi
if [[ $DOWN == "true" ]]; then
    echo -e "\033[33mFermando tutti i servizi...\033[0m"
    docker-compose down -v
    echo -e "\033[32mServizi fermati!\033[0m"
    exit 0
fi

# Visualizza i log
if [[ $LOGS == "true" ]]; then
    if [[ -n $SERVICE ]]; then
        echo -e "\033[33mVisualizzando i log del servizio: $SERVICE\033[0m"
        docker-compose logs -f "$SERVICE"
    else
        echo -e "\033[33mVisualizzando i log di tutti i servizi...\033[0m"
        docker-compose logs -f
    fi
    exit 0
fi

# Avvia i servizi
if [[ -n $SERVICE ]]; then
    echo -e "\033[33mAvviando il servizio: $SERVICE\033[0m"
else
    echo -e "\033[33mAvviando tutti i servizi...\033[0m"
fi

COMMAND="docker-compose up $DETACHED $BUILD $SERVICE"
echo -e "\033[36mComando: $COMMAND\033[0m"

if eval $COMMAND; then
    echo ""
    echo -e "\033[32m✅ Applicazione avviata con successo!\033[0m"
    echo ""
    echo -e "\033[37mAccesso ai servizi:\033[0m"
    echo -e "\033[36m  • Frontend:    http://localhost:4200\033[0m"
    echo -e "\033[36m  • Backend API: https://localhost:7086/api\033[0m"
    echo -e "\033[36m  • Swagger:     https://localhost:7086/swagger\033[0m"
    echo ""
    echo -e "\033[90mPer visualizzare i log: $0 -l\033[0m"
    echo -e "\033[90mPer fermare i servizi: $0 --down\033[0m"
else
    echo -e "\033[31m❌ Errore nell'avvio dell'applicazione!\033[0m"
    exit 1
fi 