# Banking Application - Guida Docker

Questa guida spiega come utilizzare Docker per eseguire l'applicazione Banking completa (Backend + Frontend).

## Prerequisiti

- Docker Desktop installato
- Docker Compose

## Struttura del Progetto

```
ai-fsi-demo/
├── be/                          # Backend .NET 8.0
│   ├── Banking.API/
│   ├── Banking.Models/
│   ├── Banking.Logic/
│   ├── Banking.Infrastructure/
│   └── Dockerfile
├── fe/                          # Frontend Angular 19
│   ├── src/
│   ├── Dockerfile
│   ├── nginx.conf
│   └── docker-entrypoint.sh
├── docker-compose.yml           # Orchestrazione completa
└── certs/                       # Certificati SSL (generati automaticamente)
```

## Avvio dell'Applicazione

### 1. Build e Avvio Completo

```bash
# Dalla root del progetto
docker-compose up --build
```

### 2. Avvio in Background

```bash
docker-compose up -d --build
```

### 3. Solo Backend

```bash
docker-compose up banking-api --build
```

### 4. Solo Frontend

```bash
docker-compose up banking-frontend --build
```

## Accesso ai Servizi

- **Frontend**: [http://localhost:4200](http://localhost:4200)
- **Backend API**: [https://localhost:7086/api](https://localhost:7086/api)
- **Backend HTTP**: [http://localhost:5000/api](http://localhost:5000/api)
- **Swagger**: [https://localhost:7086/swagger](https://localhost:7086/swagger)

## Certificati SSL

L'applicazione genera automaticamente certificati self-signed per HTTPS:

- Il backend crea il proprio certificato durante il build
- Il certificato viene estratto nella cartella `./certs/banking-api.crt`
- Il frontend è configurato per accettare certificati self-signed

### Installazione del Certificato (Opzionale)

Per evitare avvisi SSL nel browser:

1. Dopo il primo avvio, il certificato sarà disponibile in `./certs/banking-api.crt`
2. Installa il certificato nel tuo browser o nel sistema operativo
3. Riavvia il browser

## Configurazione

### Variabili d'Ambiente

Il docker-compose supporta le seguenti variabili:

```bash
# Per cambiare l'URL dell'API nel frontend
API_URL=https://banking-api:443/api

# Per l'ambiente del backend
ASPNETCORE_ENVIRONMENT=Production
```

### Personalizzazione

1. **Porta Frontend**: Modifica `4200:80` nel docker-compose.yml
2. **Porta Backend**: Modifica `7086:443` nel docker-compose.yml
3. **URL API**: Imposta la variabile `API_URL` nel servizio `banking-frontend`

## Gestione dei Volumi

Il backend utilizza un volume persistente:

```yaml
volumes:
  - app-data:/persistence  # Dati persistenti del backend su /persistence
```

### Backup del Volume

```bash
# Backup
docker run --rm -v ai-fsi-demo_app-data:/data -v $(pwd):/backup alpine tar czf /backup/backup.tar.gz -C /data .

# Restore
docker run --rm -v ai-fsi-demo_app-data:/data -v $(pwd):/backup alpine tar xzf /backup/backup.tar.gz -C /data
```

## Comandi Utili

### Monitoring

```bash
# Visualizza i log
docker-compose logs -f

# Visualizza i log di un singolo servizio
docker-compose logs -f banking-api
docker-compose logs -f banking-frontend

# Stato dei servizi
docker-compose ps
```

### Debugging

```bash
# Accesso al container backend
docker exec -it banking-api /bin/bash

# Accesso al container frontend
docker exec -it banking-frontend /bin/sh

# Ispezione dei volumi
docker volume inspect ai-fsi-demo_app-data
```

### Pulizia

```bash
# Ferma e rimuove i container
docker-compose down

# Rimuove anche i volumi
docker-compose down -v

# Rimuove tutto (incluse le immagini)
docker-compose down -v --rmi all
```

## Modifiche Recenti

### Problemi Risolti

1. **Swagger non disponibile in produzione**:
   - Abilitato Swagger anche in ambiente Production per i container Docker
   - Swagger ora disponibile su https://localhost:7086/swagger

2. **Volume del backend**:
   - Cambiato il volume da `/app` a `/persistence`
   - Tutti i percorsi aggiornati di conseguenza

3. **Frontend non si avvia**:
   - Aggiunto wget per healthcheck
   - Rimosso proxy nginx non necessario
   - Aggiunto endpoint `/health` per monitoraggio

4. **Healthcheck migliorati**:
   - Backend: usa endpoint `/swagger` invece di `/api`
   - Frontend: usa endpoint `/health` dedicato

## Troubleshooting

### Problemi di Certificato

1. Verifica che il certificato sia stato generato:
   ```bash
   docker exec banking-api ls -la /app/certs/
   ```

2. Rigenera i certificati:
   ```bash
   docker-compose down
   docker volume rm ai-fsi-demo_app-data
   docker-compose up --build
   ```

### Problemi di Rete

1. Verifica la connettività:
   ```bash
   docker exec banking-frontend wget -q --spider https://banking-api:443/api || echo "Connessione fallita"
   ```

2. Controlla i DNS:
   ```bash
   docker exec banking-frontend nslookup banking-api
   ```

### Test di Funzionamento

1. **Verifica Backend**:
   ```bash
   # Controlla se Swagger è accessibile
   curl -k https://localhost:7086/swagger
   
   # Testa una chiamata API
   curl -k https://localhost:7086/api/customers
   ```

2. **Verifica Frontend**:
   ```bash
   # Controlla se il frontend risponde
   curl http://localhost:4200/health
   
   # Verifica che la pagina principale si carichi
   curl http://localhost:4200
   ```

3. **Verifica Volume**:
   ```bash
   # Controlla il contenuto del volume
   docker exec banking-api ls -la /persistence/
   
   # Verifica i certificati
   docker exec banking-api ls -la /persistence/certs/
   ```

### Performance

1. **Build Cache**: Docker riutilizza le layer delle immagini. Per un rebuild completo:
   ```bash
   docker-compose build --no-cache
   ```

2. **Risorse**: Assicurati che Docker abbia risorse sufficienti (RAM > 4GB consigliata)

## Sviluppo vs Produzione

### Modalità Sviluppo

Per lo sviluppo locale senza Docker, usa:
- Backend: `dotnet run` nella cartella `be/Banking.API`
- Frontend: `npm start` nella cartella `fe`

### Modalità Produzione

Il docker-compose è configurato per la produzione con:
- Compilazione ottimizzata Angular
- HTTPS abilitato
- Certificati SSL
- Health checks
- Volumi persistenti

## Sicurezza

⚠️ **Importante**: I certificati generati sono self-signed e adatti solo per sviluppo/testing. Per produzione, usa certificati validati da una CA.

La password del certificato è hardcoded come "password" - cambiala per l'uso in produzione modificando:
- `be/Dockerfile` (variabile del certificato)
- `docker-compose.yml` (variabili d'ambiente) 