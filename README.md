# 🏦 Banking Application - Demo FSI

## Descrizione

Applicazione bancaria completa con frontend Angular e backend .NET 8, creata per dimostrare l'integrazione tra tecnologie moderne per il settore Financial Services.

## 🏗️ Architettura

```
ai-fsi-demo/
├── be/                    # Backend .NET 8
│   ├── Banking.Models/    # Modelli e Entity Framework
│   ├── Banking.Logic/     # Logica di business
│   └── Banking.API/       # Web API Controllers
└── fe/                    # Frontend Angular
    ├── src/app/models/    # Modelli TypeScript
    ├── src/app/services/  # Servizi Angular
    └── src/app/components/# Componenti UI
```

## 🚀 Funzionalità Implementate

### ✅ **Requisiti Base**
1. **Login con Tax Code**: Pagina di login che accetta il codice fiscale come username e qualsiasi password
2. **API Backend**: Endpoint che dato il Tax Code restituisce tutti gli IBAN del cliente
3. **Gestione Utenti Inesistenti**: Se non ci sono IBAN, l'app indica che l'utente è inesistente
4. **Homepage con Saldi**: Dopo il login, mostra i saldi dei conti del cliente
5. **Selezione IBAN**: Se un utente ha più IBAN, può selezionarli e vedere i saldi corrispondenti

### ✅ **Funzionalità Extra**
- **Autenticazione**: Sistema completo con AuthGuard e gestione sessioni
- **Responsive Design**: UI moderna e mobile-friendly
- **Swagger Documentation**: API documentate automaticamente
- **Error Handling**: Gestione completa degli errori
- **Loading States**: Indicatori di caricamento per migliore UX
- **Database Seeding**: Dati di test pre-caricati

## 🎯 **Utenti di Test**

### Mario Rossi
- **Tax Code**: `RSSMRA80A01H501Z`
- **Password**: qualsiasi valore
- **IBAN**: 2 conti (Corrente + Risparmio)

### Giulia Bianchi  
- **Tax Code**: `BNCGLI85M15F205W`
- **Password**: qualsiasi valore
- **IBAN**: 1 conto (Corrente)

## 🛠️ **Tecnologie Utilizzate**

### Backend (.NET 8)
- **ASP.NET Core Web API**: Framework per le API REST
- **Entity Framework Core**: ORM con database InMemory
- **Swagger/OpenAPI**: Documentazione automatica delle API
- **Dependency Injection**: Architettura a servizi

### Frontend (Angular)
- **Angular 18**: Framework SPA
- **TypeScript**: Linguaggio tipizzato
- **RxJS**: Programmazione reattiva
- **Angular Router**: Navigazione e routing
- **Reactive Forms**: Form reattivi per il login

## 📋 **API Endpoints**

### 🔐 **Login/Authentication**
```http
GET /api/customers/taxcode/{taxCode}/bank-accounts
```
Restituisce tutti i conti bancari per un dato codice fiscale (usato per il login)

### 👥 **Customers**
```http
GET /api/customers                    # Tutti i clienti
GET /api/customers/{id}               # Cliente per ID
GET /api/customers/{id}/bank-accounts # Conti di un cliente
```

### 💳 **Bank Accounts**
```http
GET /api/bankaccounts                           # Tutti i conti
GET /api/bankaccounts/iban/{iban}              # Conto per IBAN
GET /api/bankaccounts/iban/{iban}/balance      # Saldo per IBAN
GET /api/bankaccounts/balances?ibans=iban1,iban2 # Saldi multipli
```

## 🏃‍♂️ **Come Eseguire**

### 🚀 **Opzione 1: VS Code Task (Consigliato)**

Se usi **VS Code**, puoi avviare l'intera applicazione usando i task:

1. Apri il workspace in VS Code
2. Apri il **Command Palette** (Ctrl+Shift+P)
3. Digita **"Tasks: Run Task"**
4. Seleziona **"Start Full Stack"**

**Alternativa con Debug:**
1. Vai al menu **Run and Debug** (Ctrl+Shift+D)
2. Seleziona **"Full Stack (Backend + Frontend)"**
3. Premi **F5** o clicca su **Start Debugging**

Questo avvierà automaticamente:
- Backend API su `https://localhost:7086` (con profilo HTTPS)
- Frontend Angular su `http://localhost:4200` (configurato per connettersi al backend HTTPS)

La configurazione include:
- Backend configurato per usare il profilo HTTPS
- Frontend configurato con URL dell'API modificabile tramite script
- Task paralleli per avvio simultaneo dei servizi
- Configurazione separata in `fe/src/app/config/app.config.ts`

### 📝 **Opzione 2: Manuale**

#### 1. **Backend (.NET 8)**
```bash
cd be/Banking.API
dotnet run --launch-profile https
```
🌐 **URL**: `https://localhost:7086`  
📄 **Swagger**: `https://localhost:7086/swagger`

#### 2. **Frontend (Angular)**
```bash
cd fe
npm start
```
🌐 **URL**: `http://localhost:4200`

### ⚙️ **Configurazione URL API (Opzionale)**

Se devi modificare l'URL dell'API backend:

```bash
cd fe
# Imposta un URL personalizzato
npm run config:api -- -ApiUrl "https://myapi.example.com/api"

# Oppure modifica direttamente il file
# fe/src/app/config/app.config.ts
```

## 🔄 **Flusso Applicazione**

1. **Login** (`/login`):
   - Utente inserisce Tax Code + Password
   - Frontend chiama API: `GET /api/customers/taxcode/{taxCode}/bank-accounts`
   - Se 404 → "Utente inesistente"
   - Se 200 → Salva dati e naviga a `/home`

2. **Homepage** (`/home`):
   - Mostra lista IBAN del cliente
   - Se 1 IBAN → mostra direttamente il saldo
   - Se >1 IBAN → mostra selector + saldi del primo
   - Al cambio IBAN → aggiorna saldi visualizzati

## 🎨 **Caratteristiche UI/UX**

- **Design Moderno**: Gradients, shadows, animazioni
- **Responsive**: Ottimizzata per desktop e mobile
- **Loading States**: Spinner e indicatori di caricamento
- **Error Handling**: Messaggi di errore user-friendly
- **Smooth Transitions**: Animazioni fluide tra le pagine
- **Informazioni Demo**: Box con utenti di test nella login

## 📊 **Dati di Esempio**

### Clienti
| Nome | Cognome | Tax Code | Email | Telefono |
|------|---------|----------|-------|----------|
| Mario | Rossi | RSSMRA80A01H501Z | mario.rossi@email.com | 3331234567 |
| Giulia | Bianchi | BNCGLI85M15F205W | giulia.bianchi@email.com | 3339876543 |

### Conti Bancari
| IBAN | Cliente | Tipo | Saldo | Nome Conto |
|------|---------|------|-------|------------|
| IT60X0542811101000000123456 | Mario | CHECKING | €1,500.00 | Main Checking Account |
| IT60X0542811101000000654321 | Mario | SAVINGS | €5,000.00 | Savings Account |
| IT60X0542811101000000789012 | Giulia | CHECKING | €2,750.50 | Checking Account |

## 🧪 **Test delle API**

### Esempio Login Mario Rossi
```bash
curl -X GET "https://localhost:7086/api/customers/taxcode/RSSMRA80A01H501Z/bank-accounts"
```

### Esempio Saldo per IBAN
```bash
curl -X GET "https://localhost:7086/api/bankaccounts/iban/IT60X0542811101000000123456/balance"
```

### Esempio Saldi Multipli
```bash
curl -X GET "https://localhost:7086/api/bankaccounts/balances?ibans=IT60X0542811101000000123456,IT60X0542811101000000654321"
```

## ✨ **Implementazione Completata**

🎉 **L'applicazione è completamente funzionante e pronta per la demo!**

- ✅ Login funzionale con Tax Code
- ✅ Backend API .NET 8 in esecuzione
- ✅ Frontend Angular con UI moderna
- ✅ Gestione completa degli stati (loading, error, success)
- ✅ Responsive design
- ✅ Documentazione Swagger
- ✅ Dati di test pre-caricati

---

## 👨‍💻 **Sviluppato con**

**Backend**: .NET 8, ASP.NET Core, Entity Framework Core, Swagger  
**Frontend**: Angular 18, TypeScript, RxJS, SCSS  
**Database**: InMemory (Entity Framework)  
**Architettura**: Clean Architecture, Dependency Injection, RESTful APIs 

# TAG versionamento

## 0.AppAndBeReady
Applicazione pronta con dati minimi e solo saldo implementato