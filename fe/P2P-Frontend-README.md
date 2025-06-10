# Frontend P2P Transfer - Documentazione

## Panoramica

Questo documento descrive l'implementazione del frontend per i trasferimenti P2P tramite Banca Alfa nel progetto Banking Frontend Angular.

## Componenti Sviluppati

### 1. **P2PTransferComponent** (`/components/p2p-transfer`)

Il componente principale per gestire i trasferimenti P2P con le seguenti funzionalità:

#### **Funzionalità Principali:**
- ✅ **Selezione Destinatario** con rubrica interna
- ✅ **Ricerca Contatti** per nome o codice fiscale  
- ✅ **Gestione Contatti Preferiti** con stelle
- ✅ **Cronologia Trasferimenti Recenti**
- ✅ **Aggiunta Nuovi Contatti** con modal dedicato
- ✅ **Validazione Form** completa
- ✅ **Validazione API** prima del trasferimento
- ✅ **Esecuzione Trasferimenti** con feedback visivo
- ✅ **Gestione Errori** dettagliata
- ✅ **Schermata di Successo** con dettagli transazione

### 2. **ContactService** (`/services/contact.service.ts`)

Servizio per la gestione della rubrica contatti con persistenza in localStorage:

#### **Metodi Principali:**
- `getContacts()` - Ottiene tutti i contatti
- `getFavoriteContacts()` - Ottiene contatti preferiti
- `getContactsGrouped()` - Contatti raggruppati per lettera
- `getRecentTransfers()` - Cronologia trasferimenti recenti
- `addContact()` - Aggiunge nuovo contatto
- `updateContact()` - Aggiorna contatto esistente
- `deleteContact()` - Elimina contatto
- `toggleFavorite()` - Cambia stato preferito
- `searchContacts()` - Ricerca nei contatti

### 3. **Modelli TypeScript**

#### **P2P Models** (`/models/p2p.model.ts`)
```typescript
interface P2PTransferRequest {
  sender_tax_id: string;
  recipient_tax_id: string;
  amount: number;
  currency: string;
  description?: string;
  reference_id?: string;
}

interface P2PTransferResponse {
  status: string;
  transaction_id: string;
  timestamp: Date;
  amount: number;
  currency: string;
  sender: ParticipantInfo;
  recipient: ParticipantInfo;
  fees: FeeInfo;
  execution_date: Date;
  reference_id?: string;
}
```

#### **Contact Models** (`/models/contact.model.ts`)
```typescript
interface Contact {
  id: string;
  firstName: string;
  lastName: string;
  taxCode: string;
  email?: string;
  phone?: string;
  iban?: string;
  isFavorite: boolean;
  addedDate: Date;
  lastUsed?: Date;
}
```

## User Experience (UX)

### **Flusso Principale:**
1. **Home Page** → Clic su "Trasferimento P2P" 
2. **Selezione Destinatario** → Tre opzioni:
   - **Preferiti** - Contatti marcati con stella
   - **Recenti** - Ultimi trasferimenti effettuati
   - **Tutti i contatti** - Rubrica completa alfabetica
3. **Ricerca Avanzata** → Filtra per nome o codice fiscale
4. **Aggiunta Nuovo Contatto** → Modal con form completo
5. **Dettagli Trasferimento** → Importo, descrizione, ID riferimento
6. **Validazione Opzionale** → Verifica dati prima dell'invio
7. **Esecuzione** → Trasferimento con indicatori di caricamento
8. **Successo** → Schermata con dettagli completi della transazione

### **Caratteristiche UX:**
- 🎨 **Design Moderno** con gradienti e animazioni
- 📱 **Responsive** per mobile e desktop
- ⚡ **Performance** con lazy loading dei componenti
- 🔍 **Ricerca Real-time** con debounce
- 💾 **Persistenza** contatti in localStorage
- ⭐ **Preferiti** per accesso rapido
- 🕒 **Cronologia** degli ultimi trasferimenti
- ✅ **Validazione** in tempo reale
- 🎯 **Feedback Visivo** per ogni azione

## Integrazione API

### **BankingApiService** - Metodi P2P:
```typescript
// Health check servizio P2P
getP2PHealth(): Observable<HealthCheckResponse>

// Validazione richiesta trasferimento
validateP2PTransfer(request: P2PTransferRequest): Observable<ValidationResult>

// Esecuzione trasferimento P2P
executeP2PTransfer(request: P2PTransferRequest): Observable<P2PTransferResponse>
```

### **Configurazione API:**
- Base URL: `https://localhost:7086/api`
- Endpoint P2P: `/P2P/*`
- Timeout: 30 secondi
- Gestione errori automatica

## Gestione Dati Locali

### **localStorage Keys:**
- `banking-contacts` - Rubrica contatti
- `banking-recent-transfers` - Cronologia trasferimenti

### **Dati di Esempio Precaricati:**
```typescript
// 4 contatti di esempio con codici fiscali validi
Mario Rossi (RSSMRA85M01H501U) - Preferito
Giuseppe Bianchi (BNCGPP90A01F205X)
Anna Verdi (VRDNNA88H41F205Y) - Preferito  
Luigi Neri (NRELGU92B15H501Z)
```

## Validazioni Frontend

### **Codice Fiscale:**
- Pattern: `^[A-Z]{6}[0-9]{2}[A-Z][0-9]{2}[A-Z][0-9]{3}[A-Z]$`
- Normalizzazione automatica (uppercase)

### **Importo:**
- Minimo: €0.01
- Massimo: €50,000.00
- Validazione decimali

### **Campi Opzionali:**
- Descrizione: Max 200 caratteri
- Reference ID: Max 50 caratteri, pattern alfanumerico

### **IBAN (se presente):**
- Pattern: `^IT[0-9]{2}[A-Z][0-9]{22}$`

## Styling e Temi

### **Colori Principali:**
- Primary Blue: `#007bff`
- Success Green: `#28a745`
- Warning Orange: `#ffc107`
- Danger Red: `#dc3545`
- Gray Scale: `#f8f9fa` → `#343a40`

### **Componenti Stilizzati:**
- **Cards** con shadow e border-radius
- **Buttons** con hover effects e transition
- **Form Fields** con focus states
- **Modal** con overlay e animazioni
- **Loading States** con spinner
- **Toast/Alert** per feedback

## Routing

### **Nuove Rotte:**
```typescript
{
  path: 'p2p-transfer',
  loadComponent: () => import('./components/p2p-transfer/p2p-transfer.component'),
  canActivate: [AuthGuard]
}
```

### **Navigazione:**
- Da Home: Button "Trasferimento P2P" 
- Protezione: AuthGuard richiede login
- Lazy Loading: Caricamento solo quando necessario

## Testing

### **Scenari di Test:**
1. ✅ **Accesso** dalla home page
2. ✅ **Selezione** contatti preferiti
3. ✅ **Ricerca** contatti per nome/codice fiscale
4. ✅ **Aggiunta** nuovo contatto
5. ✅ **Validazione** form con dati errati
6. ✅ **Trasferimento** con successo (mock)
7. ✅ **Gestione errori** API
8. ✅ **Persistenza** dati in localStorage
9. ✅ **Responsive** design su mobile
10. ✅ **Performance** caricamento componenti

## Avvio e Build

### **Comandi Disponibili:**
```bash
# Sviluppo
npm run start:dev

# Build produzione  
npm run build:prod

# Test
npm run test
```

### **Requisiti:**
- Node.js 18+
- Angular 19
- TypeScript 5.6+

### **Configurazione API:**
File: `src/app/config/app.config.ts`
```typescript
export const appConfig: AppConfig = {
  apiUrl: 'https://localhost:7086/api',
  production: false
};
```

## Sicurezza

### **Implementazioni:**
- ✅ **AuthGuard** per proteggere le rotte
- ✅ **Validazione input** lato client
- ✅ **Sanitizzazione** dati utente
- ✅ **HTTPS** per comunicazioni API
- ✅ **localStorage** per dati non sensibili
- ✅ **Error handling** senza esporre dettagli interni

## Performance

### **Ottimizzazioni:**
- ✅ **Lazy Loading** componenti
- ✅ **OnPush** change detection strategy
- ✅ **Debounce** per ricerca real-time
- ✅ **Virtual Scrolling** per liste grandi
- ✅ **Tree Shaking** per bundle size
- ✅ **Preloading** strategico

## Roadmap Future

### **Miglioramenti Pianificati:**
- 🔄 **Sync Cloud** rubrica contatti
- 📊 **Analytics** utilizzo features
- 🔔 **Push Notifications** per transazioni
- 📱 **PWA** support offline
- 🌍 **i18n** multilingua
- 🎯 **A/B Testing** UX improvements
- 🔐 **Biometric Auth** per sicurezza
- 📈 **Dashboard** analytics trasferimenti
