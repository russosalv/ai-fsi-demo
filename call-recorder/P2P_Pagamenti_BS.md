# Pagamenti P2P

Banca Alfa  
Direzione Canali Digitali

| Metadato | Valore |
|----------|--------|
| Nome file | P2P_Pagamenti_BS |
| Classificazione | Business Specification |
| Autore | Business Manager / Business Analyst (trascrizione call) |
| Versione | 1.0 |
| Data creazione | 2025-09-08 |
| Stato | Draft |

---

## 2. CHANGE LOG
| Versione | Data | Descrizione | Autore |
|----------|------|-------------|--------|
| 1.0 | 2025-09-08 | Prima emissione (Draft) | Business Analyst |

---

## 3. INTRODUZIONE
### 3.1 Obiettivo
Definire i requisiti business e funzionali per l'erogazione della funzionalità di Pagamenti Peer-to-Peer (P2P) che consenta ai clienti di trasferire denaro in modo semplice, veloce e con elevata usabilità, mantenendo conformità normativa e adeguati livelli di sicurezza.

### 3.2 Contesto
- Banca coinvolta: Banca Alfa (integrazione con sistemi P2P di Banca Alfa) (Rif. linee 55, 118-121).
- Target clienti: Clienti retail autenticati che effettuano trasferimenti ricorrenti verso una cerchia ristretta di destinatari abituali (Rif. linee 67-68, 153-155, 157-159).
- Canali impattati: Interfaccia web responsive (desktop e mobile) (Rif. linee 86-88).
- Architettura: Frontend con lazy loading del componente P2P e integrazione via API REST (Rif. linee 103, 70-72, 118-121).
- Dati trattati: Dati anagrafici basilari dei destinatari (nome, cognome, codice fiscale) e opzionali (email, telefono, IBAN) non considerati sensibili a fini regolamentari (Rif. linee 11-16, 28, 108-109).

### 3.3 Vantaggi Attesi
- Miglioramento dell'esperienza utente grazie a rubrica, preferiti e recenti (Rif. linee 18-21, 67-68).
- Riduzione tempi operativi e incremento tasso di successo dei trasferimenti (Rif. linee 55-56, 123-124, 190-192).
- Aumento retention e soddisfazione clienti (Rif. linee 153-155, 214-216).
- Supporto a crescita futura e scalabilità (Rif. linee 168-176, 231-232).

---

## 4. REQUISITI FUNZIONALI (CORE)
Ogni sottosezione include: Descrizione, Flusso Utente, Elementi UI, Regole Business, Comportamenti Sistema, Messaggi Errore (se applicabile), Integrazioni. Riferimenti alle linee della trascrizione indicati come "Rif. lxx".

### 4.1 Rubrica & Gestione Contatti
- **Descrizione**: Rubrica personale persistita lato client (localStorage) per memorizzare contatti destinatari con campi obbligatori e opzionali. (Rif. 10-16)
- **Flusso Utente**:
  1. L'utente accede alla sezione P2P.
  2. Visualizza sezioni Preferiti, Recenti, Tutti i contatti. (Rif. 18-21)
  3. Può selezionare un contatto esistente o crearne uno nuovo. (Rif. 25-28)
- **Elementi UI**: Liste "Preferiti", "Recenti", "Tutti i contatti" con organizzazione alfabetica. (Rif. 19-21)
- **Regole Business**:
  - Campi obbligatori contatto: Nome, Cognome, Codice Fiscale. (Rif. 27-28)
  - Campi opzionali: Email, Telefono, IBAN. (Rif. 12, 28)
  - Flag Preferito gestito a livello contatto. (Rif. 19, 112-113)
  - Aggiornamento automatico data ultimo utilizzo dopo trasferimento completato. (Rif. 65)
- **Comportamenti Sistema**:
  - Persistenza locale in localStorage (non dati sensibili). (Rif. 14-16,109)
  - Aggiornamento liste dinamico post-salvataggio/trasferimento.
- **Integrazioni**: Nessuna chiamata server per rubrica (fase attuale). (Rif. 14)
- **Note Future (non requisito attuale)**: Sincronizzazione cloud rubrica. (Rif. 137)

### 4.2 Selezione Destinatario
- **Descrizione**: Meccanismo di scelta rapida tramite Preferiti, Recenti o elenco completo alfabetico. (Rif. 18-21)
- **Flusso Utente**:
  1. Visualizza schede/accordion delle tre categorie.
  2. Seleziona contatto; visualizza dettagli prima del trasferimento. (Rif. 37-39)
- **Elementi UI**: Tab/Sezioni: Preferiti, Recenti, Tutti. Card contatto con Nome, Codice Fiscale, stato preferito, ultimo utilizzo, IBAN se presente.
- **Regole Business**:
  - Recenti: ultimi 10 trasferimenti. (Rif. 64)
  - Preferiti: sottoinsieme marcato manualmente/precaricato. (Rif. 19, 112-113)
  - Tutti: ordine alfabetico. (Rif. 21)
- **Comportamenti Sistema**: Refresh immediate di Preferiti e Recenti dopo esecuzione trasferimento. (Rif. 63-65)
- **Integrazioni**: Nessuna (dati locali) nella fase attuale.

### 4.3 Ricerca Real-time Contatti
- **Descrizione**: Ricerca filtrante per Nome o Codice Fiscale con risposta reattiva e non gravosa. (Rif. 22-24)
- **Flusso Utente**: Inserimento query nel campo di ricerca; elenco si aggiorna dopo debounce. (Rif. 105)
- **Elementi UI**: Input testo con placeholder (es. "Cerca per nome o codice fiscale"). Indicatore caricamento opzionale se necessario.
- **Regole Business**:
  - Matching case-insensitive.
  - Ambito ricerca: rubrica locale.
  - Debounce 300ms. (Rif. 105)
- **Comportamenti Sistema**: Applica filtro post debounce; nessuna chiamata remota.

### 4.4 Aggiunta Nuovo Contatto
- **Descrizione**: Creazione contatto via form modale attraverso pulsante "+ Nuovo". (Rif. 26-28)
- **Flusso Utente**: Click "+ Nuovo" -> apertura form -> compilazione campi -> validazione frontend -> salvataggio locale -> aggiornamento liste.
- **Elementi UI**: Pulsante primario "+ Nuovo"; Form con campi (Nome, Cognome, Codice Fiscale, Email?, Telefono?, IBAN?). Submit/Annulla.
- **Regole Business**: Obbligatori vs opzionali come sezione 4.1. Uppercase automatico Codice Fiscale e IBAN. (Rif. 32, 35)
- **Comportamenti Sistema**: Validazioni in tempo reale sui campi; mostra errori granulari; disabilita salvataggio se errori.

### 4.5 Validazioni Codice Fiscale & IBAN
- **Descrizione**: Controlli formali su formati identificativi. (Rif. 30-35)
- **Regole Business**:
  - Codice Fiscale pattern: 6 lettere + 2 numeri + 1 lettera + 2 numeri + 1 lettera + 3 numeri + 1 lettera (es. RSSMRA85M01H501U). (Rif. 31-32)
  - IBAN italiano: "IT" + 25 caratteri alfanumerici (es. IT60X0542811101000000123456). (Rif. 34-35)
  - Auto-conversione a maiuscolo. (Rif. 32, 35)
- **Comportamenti Sistema**: Validazione frontend immediata; errori bloccanti salvataggio/trasferimento.
- **Messaggi Errore**: Esempio: "Il codice fiscale inserito non è valido" (Rif. 99). Analogo per IBAN: "L'IBAN inserito non è valido" (derivato pattern). Solo frasi chiare non tecniche. (Rif. 99-100)

### 4.6 Flusso Trasferimento
- **Descrizione**: Processo dall'avvenuta selezione destinatario alla conferma esito. (Rif. 37-44, 48-61)
- **Flusso Utente**:
  1. Seleziona destinatario; visualizza dettagli (Nome, Codice Fiscale, IBAN). (Rif. 37-39)
  2. Compila form trasferimento: Importo, Descrizione (opzionale), ID Riferimento (opzionale). (Rif. 44-45)
  3. Pulsanti disponibili: "Valida" (opzionale) e "Trasferisci". (Rif. 48-49)
  4. Se sceglie "Valida": chiamata API validazione; feedback eventuali errori. (Rif. 50-53)
  5. Se sceglie "Trasferisci": invio richiesta esecuzione; stato elaborazione; risultato (successo o errore). (Rif. 55-60)
- **Elementi UI**: Card Dettaglio Destinatario; Form trasferimento; Pulsanti; Indicatori stato (spinner, testo "Trasferimento..."). (Rif. 56)
- **Regole Business**:
  - Pulsante "Trasferisci" disabilitato finché esistono errori validazione. (Rif. 53)
  - Importo nei limiti (vedi 4.7). (Rif. 41)
- **Comportamenti Sistema**:
  - Blocco interazioni durante elaborazione. (Rif. 57)
  - Aggiornamento cronologia e ultimo utilizzo contatto dopo successo. (Rif. 63-65)
- **Integrazioni**: API validate (opzionale) ed execute. (Rif. 70-72)

### 4.7 Limiti Importo & Campi Trasferimento
- **Descrizione**: Vincoli e caratteristiche dei campi monetari e di descrizione. (Rif. 41, 44-45)
- **Regole Business**:
  - Importo minimo: 0,01 EUR; massimo: 50.000 EUR. (Rif. 41)
  - Descrizione: opzionale, max 200 caratteri, contatore caratteri in tempo reale. (Rif. 44, 46)
  - ID Riferimento: opzionale, max 50 caratteri. (Rif. 45)
  - Valuta: indicata come field "currency" nelle API (Rif. 74-75) - assumere EUR se non diversamente specificato (non esteso oltre call).
- **Comportamenti Sistema**: Validazione realtime; messaggi su quantità fuori range (Rif. 42, 95).

### 4.8 Pulsanti "Valida" e "Trasferisci"
- **Descrizione**: Controlli primari di interazione per pre-verifica e invio esecuzione. (Rif. 48-55)
- **Regole Business**:
  - "Valida" opzionale; esegue chiamata validazione dati. (Rif. 49-50)
  - Se errori backend: mostra messaggi e non abilita esecuzione fino a risoluzione. (Rif. 51-53)
  - "Trasferisci" esegue operazione; durante elaborazione testo sostituito e disabilitato. (Rif. 56-57)
- **Comportamenti Sistema**: Stato loading centralizzato; prevenzione azioni duplicate.

### 4.9 Gestione Errori
- **Descrizione**: Classificazione e gestione multilivello errori. (Rif. 94-101)
- **Categorie**:
  - Frontend validation: formato codice fiscale, importi fuori range, IBAN non valido. (Rif. 95)
  - Backend validation: conto inesistente, fondi insufficienti. (Rif. 96)
  - Sistema: connessione, timeout. (Rif. 97)
- **Messaggi**: Linguaggio non tecnico, chiaro. (Rif. 99-100)
- **Comportamenti Sistema**: Toast e indicazioni inline (Rif. 90-92); disabilitazione pulsante "Trasferisci" con errori. (Rif. 53)

### 4.10 Cronologia "Recenti"
- **Descrizione**: Lista ultimi trasferimenti completati. (Rif. 63-65)
- **Regole Business**:
  - Conservare ultimi 10 trasferimenti con importo, data, destinatario. (Rif. 64)
  - Aggiornare data ultimo utilizzo contatto. (Rif. 65)
- **Comportamenti Sistema**: Inserimento in testa alla lista; rimozione entry più vecchia oltre limite.

### 4.11 Schermata Successo Trasferimento
- **Descrizione**: Conferma visiva esito positivo. (Rif. 59-61)
- **Elementi UI**: Icona spunta, dettagli: ID transazione, importo, destinatario, data/ora, commissioni se presenti. (Rif. 60-61, 77-81, 82-85)
- **Comportamenti Sistema**: Visualizzazione pulsanti "Nuovo Trasferimento" e "Torna alla Home". (Rif. 61)

### 4.12 Analytics & Tracking
- **Descrizione**: Raccolta eventi principali per monitoraggio uso e performance. (Rif. 128-135, 129-131, 133-135)
- **Regole Business**:
  - Eventi: avvio trasferimento, completamento, errore. (Rif. 129)
  - Tracciare modalità selezione contatto (Preferiti, Recenti, Ricerca). (Rif. 130-131)
  - Distribuzione uso (baseline: 60% Preferiti, 25% Recenti, 15% Ricerca). (Rif. 133-135)

### 4.13 Dati Test Precaricati
- **Descrizione**: Set di contatti di esempio per demo. (Rif. 111-116)
- **Contenuto**: 4 contatti con codici fiscali validi; due marcati come preferiti; campi completi per tutti. (Rif. 111-116)

### 4.14 Roadmap (Informazioni Non Scope Attuale)
- Sincronizzazione rubrica multi-dispositivo. (Rif. 137)
- Push notification conferme. (Rif. 138)
- Autenticazione biometrica. (Rif. 139)
- Internazionalizzazione e validazioni multi-paese. (Rif. 141-142)
- Pagamenti ricorrenti programmati. (Rif. 223-226)
- Integrazione contatti telefono (valutazione privacy). (Rif. 224, 227)

---

## 5. SPECIFICHE TECNICHE
### 5.1 Canali Coinvolti
Interfaccia web responsive desktop/mobile. (Rif. 86-88)

### 5.2 Profili Utente
Clienti autenticati (accesso protetto da AuthGuard). (Rif. 108)

### 5.3 Sicurezza
- HTTPS obbligatorio. (Rif. 72, 107)
- Sanitizzazione input contro XSS. (Rif. 107)
- AuthGuard per rotte P2P. (Rif. 108)
- Nessun dato sensibile in localStorage. (Rif. 109)
- Conformità PSD2, GDPR, limiti normativi trasferimenti. (Rif. 202-205)
- Penetration test e audit periodici. (Rif. 206-208)

### 5.4 Performance
- Lazy loading componente P2P. (Rif. 103)
- Virtual scrolling rubrica grandi dimensioni. (Rif. 104)
- Debounce ricerca 300ms. (Rif. 105)
- Tempo risposta API < 5s nel 95% dei casi (baseline <3s). (Rif. 123-124, 190-191)
- Scalabilità 10x volume attuale. (Rif. 175-176)

### 5.5 Integrazioni API
- Endpoint health check. (Rif. 70-71, 125-127)
- Endpoint validazione dati. (Rif. 71)
- Endpoint esecuzione trasferimento. (Rif. 71, 55)
- Formato request: sender_tax_id, recipient_tax_id, amount, currency, description?, reference_id? (Rif. 74-75)
- Formato response: status, transaction_id, timestamp, sender/recipient (tax_id, account_iban), execution_date, fees. (Rif. 77-81)
- Gestione indisponibilità: messaggio servizio non disponibile e disabilitazione funzionalità. (Rif. 125-127)

### 5.6 Monitoring & Metriche
- Tempi risposta API P2P. (Rif. 190-191)
- Tasso successo trasferimenti. (Rif. 191)
- Uptime servizio. (Rif. 192)
- Eventi analytics (vedi 4.12). (Rif. 129-131)

### 5.7 Deployment & Operatività
- Containerizzazione (Docker). (Rif. 186-187)
- CI/CD automatizzata con test. (Rif. 187)
- Blue-green deployment, feature flag, rollback <2 minuti. (Rif. 194-196)
- Monitoring real-time performance & availability. (Rif. 188)

### 5.8 Compliance
- PSD2, GDPR, limiti importo. (Rif. 202-205, 41)
- Audit sicurezza e compliance periodici. (Rif. 206-208)

### 5.9 Scalabilità & Resilienza
- Architettura pronta per aumento volume e nuove funzionalità. (Rif. 168-176, 231-232)
- Endpoint health per abilitazione/disabilitazione proattiva. (Rif. 125-127)

---

## 6. TABELLE DETTAGLIO UI
### 6.1 Schermata Selezione Destinatario
| Sezione | Elemento | Tipologia | Descrizione |
|---------|----------|-----------|-------------|
| Header | Titolo "Pagamenti P2P" | Testo | Identifica funzionalità |
| Rubrica | Tab Preferiti | Tab | Elenco contatti marcati preferiti (Rif. 19) |
| Rubrica | Tab Recenti | Tab | Ultimi 10 trasferimenti (Rif. 64) |
| Rubrica | Tab Tutti i Contatti | Tab | Elenco alfabetico (Rif. 21) |
| Ricerca | Campo ricerca | Input | Filtro per nome o codice fiscale con debounce 300ms (Rif. 22-24, 105) |
| Contatti | Card contatto | Card | Nome, CF, IBAN (se esiste), icona preferito |
| Azioni | Pulsante + Nuovo | CTA | Apre form creazione contatto (Rif. 26) |

### 6.2 Modal Nuovo Contatto
| Sezione | Elemento | Tipologia | Descrizione |
|---------|----------|-----------|-------------|
| Form | Nome | Input | Obbligatorio (Rif. 27) |
| Form | Cognome | Input | Obbligatorio (Rif. 27) |
| Form | Codice Fiscale | Input | Obbligatorio, uppercase, pattern (Rif. 30-32) |
| Form | Email | Input | Opzionale (Rif. 12, 28) |
| Form | Telefono | Input | Opzionale (Rif. 12, 28) |
| Form | IBAN | Input | Opzionale, uppercase, pattern IT + 25 (Rif. 34-35) |
| Azioni | Salva | CTA | Salvataggio se validazioni superate |
| Azioni | Annulla | CTA | Chiude senza salvare |
| Feedback | Messaggi errore campi | Testo | Chiarezza non tecnica (Rif. 99-100) |

### 6.3 Form Trasferimento
| Sezione | Elemento | Tipologia | Descrizione |
|---------|----------|-----------|-------------|
| Destinatario | Card dettaglio | Card | Nome, CF, IBAN modificabile (Rif. 37-39) |
| Importo | Campo importo | Input | Min 0,01 Max 50.000 EUR (Rif. 41) |
| Descrizione | Campo descrizione | Textarea | Opzionale, max 200, contatore (Rif. 44, 46) |
| Riferimento | Campo ID Riferimento | Input | Opzionale, max 50 (Rif. 45) |
| Validazione | Pulsante Valida | CTA | Chiamata API validazione (Rif. 48-50) |
| Esecuzione | Pulsante Trasferisci | CTA | Esegue trasferimento (Rif. 55) |
| Stato | Spinner/Testo | Feedback | "Trasferimento..." durante elaborazione (Rif. 56-57) |
| Errori | Messaggi validazione | Testo | Frontend, backend, sistema (Rif. 94-101) |

### 6.4 Schermata Successo
| Sezione | Elemento | Tipologia | Descrizione |
|---------|----------|-----------|-------------|
| Esito | Icona spunta | Icona | Conferma successo (Rif. 59) |
| Dettagli | ID transazione | Testo | transaction_id (Rif. 60, 77) |
| Dettagli | Importo | Testo | Valore trasferito |
| Dettagli | Destinatario | Testo | Nome + CF |
| Dettagli | Data/Ora | Testo | Timestamp execution_date (Rif. 77-80) |
| Dettagli | Commissioni | Testo | Fees se presenti (Rif. 80-85) |
| Azioni | Nuovo Trasferimento | CTA | Riavvia flusso (Rif. 61) |
| Azioni | Torna alla Home | CTA | Navigazione uscita (Rif. 61) |

### 6.5 Toast / Errori
| Sezione | Elemento | Tipologia | Descrizione |
|---------|----------|-----------|-------------|
| Feedback | Toast informativi | Notifica | Stato operazioni (Rif. 92) |
| Feedback | Errori validazione | Testo inline | Formato non valido (Rif. 95-99) |
| Feedback | Errori backend | Toast + inline | Fondi insufficienti, conto inesistente (Rif. 96) |
| Feedback | Errori sistema | Toast persistente | Connessione/timeout (Rif. 97) |

### 6.6 Stato Servizio Non Disponibile
| Sezione | Elemento | Tipologia | Descrizione |
|---------|----------|-----------|-------------|
| Health | Messaggio servizio non disponibile | Banner | Mostrato se /health fallisce (Rif. 125-127) |
| Azioni | Disabilitazione trasferimento | Stato | Blocca pulsanti operativi (Rif. 127) |

### 6.7 Cronologia Recenti
| Sezione | Elemento | Tipologia | Descrizione |
|---------|----------|-----------|-------------|
| Lista | Ultimi trasferimenti | Lista | Ultimi 10 con importo/data/destinatario (Rif. 64) |
| Item | Singolo trasferimento | Riga | Importo, data, destinatario |

---

## 7. TRACCIABILITÀ REQUISITI
| ID | Descrizione Requisito | Sezione Documento | Riferimento SRT |
|----|-----------------------|-------------------|-----------------|
| R1 | Rubrica locale con campi obbligatori/opzionali | 4.1 | 10-16, 27-28 |
| R2 | Categorie Preferiti, Recenti, Tutti | 4.1, 4.2 | 18-21 |
| R3 | Ricerca realtime debounce 300ms | 4.3 | 22-24, 105 |
| R4 | Aggiunta nuovo contatto via form | 4.4 | 26-28 |
| R5 | Validazione CF pattern | 4.5 | 30-32 |
| R6 | Validazione IBAN italiano | 4.5 | 34-35 |
| R7 | Uppercase automatico CF/IBAN | 4.5 | 32, 35 |
| R8 | Limiti importo 0,01 - 50.000 EUR | 4.7 | 41 |
| R9 | Campi opzionali trasferimento (descrizione, ID riferimento) | 4.7 | 44-46 |
| R10 | Pulsante Valida opzionale | 4.8 | 48-50 |
| R11 | Disabilitare Trasferisci con errori | 4.6, 4.8 | 53 |
| R12 | Gestione errori 3 livelli | 4.9 | 94-101 |
| R13 | Cronologia ultimi 10 trasferimenti | 4.10 | 63-65 |
| R14 | Schermata successo con dettagli transazione | 4.11 | 59-61, 77-81 |
| R15 | Analytics eventi e modalità selezione | 4.12 | 128-135 |
| R16 | Dati test precaricati | 4.13 | 111-116 |
| R17 | Sicurezza HTTPS, sanitizzazione, AuthGuard | 5.3 | 72, 107-109 |
| R18 | Endpoint health, validate, execute | 5.5 | 70-72, 125-127 |
| R19 | Performance lazy load, virtual scroll, debounce | 5.4 | 103-105 |
| R20 | Tempo risposta <5s 95% | 5.4 | 190-191 |
| R21 | Scalabilità 10x | 5.9 | 175-176 |
| R22 | Compliance PSD2/GDPR | 5.8 | 202-205 |
| R23 | Monitoring metriche chiave | 5.6 | 190-192 |
| R24 | Blue-green deployment e rollback rapido | 5.7 | 194-196 |
| R25 | Fees visualizzate se presenti | 4.11 | 80-85 |
| R26 | Aggiornamento ultimo utilizzo contatto | 4.1, 4.10 | 65 |
| R27 | Messaggi non tecnici | 4.9 | 99-100 |
| R28 | Blocco UI durante elaborazione | 4.6, 4.8 | 56-57 |
| R29 | Contatore caratteri descrizione | 4.7 | 46 |
| R30 | Feature flag future estensioni | 5.7 | 195-196 |

---

## 8. ASSUNZIONI E LIMITI (Derivate dalla Call)
| ID | Assunzione | Motivazione |
|----|------------|-------------|
| A1 | Valuta operazioni EUR | Nessuna altra valuta menzionata (Rif. 74-75) |
| A2 | Persistenza solo client-side fase iniziale | Richiesta semplicità (Rif. 14-16) |
| A3 | Dati rubrica non sensibili | Chiarito (Rif. 16, 109) |
| A4 | Numero contatti gestibile, ottimizzazioni previste | Virtual scroll menzionato come eventuale (Rif. 104) |

---

## 9. FUORI SCOPO (Fase Attuale)
- Sincronizzazione cloud rubrica (Rif. 137)
- Notifiche push (Rif. 138)
- Autenticazione biometrica (Rif. 139)
- Internationalizzazione e codici fiscali/IBAN esteri (Rif. 141-142)
- Pagamenti ricorrenti programmati (Rif. 223-226)
- Integrazione contatti telefono (Rif. 224, 227)

---

## 10. APPENDICE
### 10.1 Glossario
| Termine | Definizione |
|---------|-------------|
| P2P | Pagamenti Peer-to-Peer tra clienti della banca |
| Preferiti | Contatti marcati per uso frequente |
| Recenti | Ultimi 10 trasferimenti completati |
| Rubrica | Elenco contatti memorizzati lato client |
| Fees | Commissioni applicate dal backend |
| Health Check | Verifica disponibilità servizio P2P |

### 10.2 Riferimenti
- Trascrizione call requisiti P2P (file SRT)

---

Documento generato sulla base esclusiva dei contenuti presenti nella trascrizione SRT, senza aggiunta di elementi estranei alla call.