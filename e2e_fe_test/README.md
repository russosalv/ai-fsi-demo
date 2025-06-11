# Test E2E Frontend Banking Portal

Questa cartella contiene i test End-to-End automatici per il Banking Portal, sviluppati con **Puppeteer** e ottimizzati per l'esecuzione in pipeline **Azure DevOps**.

## 🎯 Obiettivo

Il test replica esattamente il flusso utente critico:
1. **Navigazione** al frontend `http://localhost:4200` con viewport mobile
2. **Login** con utente demo Mario Rossi (`RSSMRA80A01H501Z` / `1234`)  
3. **Click** sull'icona "Trasferimento P2P"
4. **Verifica** che l'URL finale sia `http://localhost:4200/p2p-transfer`

## 📁 Struttura File

```
e2e_fe_test/
├── banking-portal-e2e.js     # Test principale Puppeteer
├── package.json              # Dipendenze Node.js
├── azure-pipelines-e2e.yml   # Configurazione Azure DevOps
└── README.md                 # Questa documentazione
```

## 🚀 Esecuzione Locale

### Prerequisiti
- **Node.js** 18+ installato
- **Frontend** in esecuzione su `http://localhost:4200`
- **Backend** in esecuzione (solitamente porta 5000)

### Installazione Dipendenze
```bash
cd e2e_fe_test
npm install
```

### Esecuzione Test

**Modalità con interfaccia grafica (sviluppo):**
```bash
npm test
# oppure
node banking-portal-e2e.js
```

**Modalità headless (CI/CD):**
```bash
npm run test:headless
# oppure  
node banking-portal-e2e.js --headless
```

## 🔧 Configurazione

### Parametri Configurabili

Il file `banking-portal-e2e.js` contiene una sezione `CONFIG` facilmente modificabile:

```javascript
const CONFIG = {
  BASE_URL: 'http://localhost:4200',           // URL frontend
  DEMO_USER: {
    fiscalCode: 'RSSMRA80A01H501Z',            // Codice fiscale demo
    password: '1234'                           // Password demo
  },
  VIEWPORT: { width: 375, height: 812 },       // Dimensioni mobile
  TIMEOUTS: {
    navigation: 30000,                         // Timeout navigazione
    elementWait: 10000                         // Timeout elementi
  },
  EXPECTED_P2P_URL: 'http://localhost:4200/p2p-transfer'  // URL atteso
};
```

### Codici di Uscita

- **Exit Code 1**: Test completato con **SUCCESSO** ✅
- **Exit Code 0**: Test **FALLITO** ❌

> **Nota**: Questa convenzione è specifica per Azure DevOps che interpreta exit code 0 come fallimento nei test.

## 🔄 Azure DevOps Pipeline

### Setup Pipeline

1. **Copia** il file `azure-pipelines-e2e.yml` nella root del repository
2. **Crea** una nuova pipeline in Azure DevOps puntando a questo file
3. **Configura** le variabili se necessario:
   - `nodeVersion`: versione Node.js (default: 18.x)
   - `frontendPort`: porta frontend (default: 4200)  
   - `backendPort`: porta backend (default: 5000)

### Trigger Automatici

La pipeline si attiva automaticamente su:
- **Push** sui branch `main` e `develop`
- **Modifiche** alle cartelle `fe/*` e `e2e_fe_test/*`

### Processo Pipeline

1. ⚙️ **Setup** ambiente (Node.js, .NET)
2. 🔨 **Build** backend e frontend
3. 🚀 **Avvio** servizi in background
4. ⏱️ **Attesa** che i servizi siano pronti
5. 🧪 **Esecuzione** test E2E
6. 🧹 **Cleanup** processi
7. 📋 **Pubblicazione** log/screenshot se fallimenti

## 🐛 Debugging

### Screenshot Automatici

In caso di errore, il test genera automaticamente screenshot:
- `error-[step]-[timestamp].png`: Screenshot al momento dell'errore
- `final-error-[timestamp].png`: Screenshot finale

### Log Dettagliati

Il test produce log timestampati per ogni operazione:
```
[2025-06-11T10:15:30.123Z] INFO: 🚀 Avvio test E2E Banking Portal
[2025-06-11T10:15:31.456Z] INFO: Step 1: Navigazione alla pagina di login...
[2025-06-11T10:15:33.789Z] INFO: ✅ Navigazione completata con successo
...
```

### Problemi Comuni

**❌ Errore "Elemento non trovato"**
- Verificare che frontend e backend siano avviati
- Controllare che l'utente demo sia configurato correttamente
- Aumentare i timeout in caso di ambienti lenti

**❌ Errore di navigazione**  
- Verificare che `http://localhost:4200` sia raggiungibile
- Controllare la configurazione di rete/firewall

**❌ Screenshot vuoti**
- Il browser potrebbe non essere visibile in modalità headless
- Verificare i permessi di scrittura nella cartella

## 🔒 Sicurezza

- **Credenziali**: Utilizzare solo utenti demo/test
- **Rete**: Test progettati per ambiente localhost
- **Isolamento**: Ogni esecuzione utilizza un browser pulito

## 📈 Metriche & Monitoring

Il test riporta automaticamente:
- ⏱️ **Durata** totale esecuzione
- 📊 **Step** completati vs falliti  
- 🖼️ **Screenshot** per analisi visiva
- 📝 **Log** dettagliati per debugging

## 🤝 Contributi

Per modificare o estendere i test:

1. **Modifica** `banking-portal-e2e.js` per nuovi step
2. **Aggiorna** `CONFIG` per nuove configurazioni
3. **Testa** localmente prima del commit
4. **Documenta** eventuali nuovi parametri in questo README

---

**Developed with ❤️ for Banking Portal E2E Testing** 