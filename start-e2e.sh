#!/bin/bash

# Script per avviare Backend e Frontend per test End-to-End
# Uso: ./start-e2e.sh

set -e

# Colori per l'output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
GRAY='\033[0;37m'
NC='\033[0m' # No Color

# PID dei processi
BACKEND_PID=""
FRONTEND_PID=""

# Funzione per pulire i processi quando lo script viene terminato
cleanup() {
    echo -e "\n${YELLOW}🧹 Pulizia processi in corso...${NC}"
    
    if [ ! -z "$BACKEND_PID" ]; then
        echo -e "${YELLOW}Terminazione Backend (PID: $BACKEND_PID)...${NC}"
        kill -TERM "$BACKEND_PID" 2>/dev/null || true
        wait "$BACKEND_PID" 2>/dev/null || true
    fi
    
    if [ ! -z "$FRONTEND_PID" ]; then
        echo -e "${YELLOW}Terminazione Frontend (PID: $FRONTEND_PID)...${NC}"
        kill -TERM "$FRONTEND_PID" 2>/dev/null || true
        wait "$FRONTEND_PID" 2>/dev/null || true
    fi
    
    # Termina tutti i processi dotnet e ng serve rimanenti
    pkill -f "dotnet.*Banking.API" 2>/dev/null || true
    pkill -f "ng serve" 2>/dev/null || true
    
    echo -e "${GREEN}✅ Pulizia completata!${NC}"
    exit 0
}

# Registra il gestore per SIGINT (Ctrl+C) e SIGTERM
trap cleanup SIGINT SIGTERM EXIT

echo -e "${GREEN}🚀 Avvio Backend e Frontend per test E2E...${NC}"

# Verifica che le directory esistano
if [ ! -d "be/Banking.API" ]; then
    echo -e "${RED}❌ Directory Backend non trovata: be/Banking.API${NC}"
    exit 1
fi

if [ ! -d "fe" ]; then
    echo -e "${RED}❌ Directory Frontend non trovata: fe${NC}"
    exit 1
fi

# Verifica che dotnet sia installato
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}❌ .NET CLI non trovato. Installa .NET SDK${NC}"
    exit 1
fi

# Verifica che npm sia installato
if ! command -v npm &> /dev/null; then
    echo -e "${RED}❌ npm non trovato. Installa Node.js${NC}"
    exit 1
fi

# Verifica che Angular CLI sia installato
if ! command -v npx &> /dev/null || ! npx ng version &> /dev/null; then
    echo -e "${RED}❌ Angular CLI non trovato. Installa con: npm install -g @angular/cli${NC}"
    exit 1
fi

echo -e "${CYAN}🔧 Controllo dipendenze frontend...${NC}"
cd fe
if [ ! -d "node_modules" ]; then
    echo -e "${CYAN}📦 Installazione dipendenze npm...${NC}"
    npm install
    if [ $? -ne 0 ]; then
        echo -e "${RED}❌ Errore durante l'installazione delle dipendenze npm${NC}"
        cd ..
        exit 1
    fi
fi
cd ..

echo -e "${CYAN}🌐 Avvio Backend (.NET API)...${NC}"

# Avvia il backend in background
cd be/Banking.API
export ASPNETCORE_ENVIRONMENT=Development
dotnet run --launch-profile https > ../../backend.log 2>&1 &
BACKEND_PID=$!
cd ../..

echo -e "${YELLOW}⏳ Attesa avvio Backend (PID: $BACKEND_PID)...${NC}"
sleep 10

# Verifica che il backend sia ancora in esecuzione
if ! kill -0 "$BACKEND_PID" 2>/dev/null; then
    echo -e "${RED}❌ Backend è terminato con errore! Controlla backend.log${NC}"
    cat backend.log
    exit 1
fi

echo -e "${CYAN}🎨 Avvio Frontend (Angular)...${NC}"

# Avvia il frontend in background
cd fe
export BACKEND_API_URL="https://localhost:7086/api"
npx ng serve --port 4200 --host 0.0.0.0 > ../frontend.log 2>&1 &
FRONTEND_PID=$!
cd ..

echo -e "${YELLOW}⏳ Attesa avvio Frontend (PID: $FRONTEND_PID)...${NC}"
sleep 15

# Verifica che il frontend sia ancora in esecuzione
if ! kill -0 "$FRONTEND_PID" 2>/dev/null; then
    echo -e "${RED}❌ Frontend è terminato con errore! Controlla frontend.log${NC}"
    cat frontend.log
    exit 1
fi

echo -e "${GREEN}✅ Servizi avviati!${NC}"
echo -e "${CYAN}🌐 Backend: https://localhost:7086${NC}"
echo -e "${CYAN}🎨 Frontend: http://localhost:4200${NC}"
echo ""
echo -e "${NC}📋 Stato servizi:${NC}"
echo -e "   ${GRAY}Backend PID: $BACKEND_PID${NC}"
echo -e "   ${GRAY}Frontend PID: $FRONTEND_PID${NC}"
echo ""
echo -e "${GREEN}🧪 Ora puoi eseguire i tuoi test E2E!${NC}"
echo -e "${YELLOW}⚠️  Premi Ctrl+C per terminare entrambi i servizi${NC}"
echo -e "${GRAY}📄 Log disponibili in: backend.log e frontend.log${NC}"
echo ""

# Mantieni lo script in esecuzione e monitora i processi
while true; do
    # Controlla se i processi sono ancora in esecuzione
    if ! kill -0 "$BACKEND_PID" 2>/dev/null; then
        echo -e "${RED}❌ Backend è terminato inaspettatamente!${NC}"
        echo -e "${GRAY}Ultimi log del backend:${NC}"
        tail -20 backend.log
        break
    fi
    
    if ! kill -0 "$FRONTEND_PID" 2>/dev/null; then
        echo -e "${RED}❌ Frontend è terminato inaspettatamente!${NC}"
        echo -e "${GRAY}Ultimi log del frontend:${NC}"
        tail -20 frontend.log
        break
    fi
    
    sleep 5
done 