### 1. HEADER

# Pagamenti P2P

Banca/Organizzazione: N/D (integrazione con Banca Alfa)
Direzione responsabile: N/D

| Metadato | Valore |
|----------|--------|
| Nome file | Pagamenti_P2P_BS |
| Classificazione | Business Specification |
| Autore | Business Manager (Utente1); Business Analyst (Utente2) |
| Versione | 1.0 |
| Data creazione | 2025-09-08 |
| Stato | Draft |

### 2. CHANGE LOG

| Versione | Data | Autore | Descrizione |
|----------|------|--------|-------------|
| 1.0 | 2025-09-08 | Utente1; Utente2 | Prima emissione del documento |

### 3. INTRODUZIONE

- **Obiettivo**: Definire in maniera completa e non ambigua i requisiti di business per la funzionalità di pagamenti P2P, incluse rubrica contatti, selezione destinatario, validazione, esecuzione trasferimento, gestione esiti, cronologia e aspetti di UX, performance, sicurezza, compliance e monitoraggio. (Rif. SRT L11-L19, L279-L287)
- **Contesto**:
  - **Banche coinvolte**: Integrazione con servizi P2P di Banca Alfa tramite backend proprietario. (Rif. SRT L471-L480)
  - **Target clienti**: Clienti retail della banca (utilizzo ricorrente verso contatti abituali). (Rif. SRT L267-L273)
  - **Canali impattati**: Canale digitale web responsive (desktop e mobile). (Rif. SRT L343-L351)
  - **Vantaggi attesi**: Rapidità e semplicità d’uso, interfaccia moderna, vantaggio competitivo grazie a rubrica integrata e tempi di risposta elevati. (Rif. SRT L343-L351, L595-L603)

### 4. REQUISITI FUNZIONALI (CORE)

#### 4.1 Rubrica Contatti P2P
- **Descrizione**: Rubrica personale integrata per salvare e gestire contatti beneficiari. Campi: nome, cognome, codice fiscale (obbligatori); email, telefono, IBAN (opzionali). Persistenza locale tramite localStorage. (Rif. SRT L39-L55, L63-L67)
- **Flusso Utente**:
  1. Accesso alla sezione P2P e apertura rubrica.
  2. Visualizzazione liste: Preferiti, Recenti, Tutti i contatti.
  3. Selezione del destinatario dalla lista o ricerca. (Rif. SRT L71-L84)
- **Elementi UI**:
  - Tab/Sezioni: "Preferiti", "Recenti", "Tutti i contatti" ordinate alfabeticamente per "Tutti". (Rif. SRT L75-L84)
  - Card contatto con nome, cognome, codice fiscale, eventuale IBAN/email/telefono; indicatore preferito. (Rif. SRT L39-L48, L75-L76)
  - Persistenza locale (localStorage). (Rif. SRT L55-L60)
- **Regole Business**:
  - Campi obbligatori: nome, cognome, codice fiscale. (Rif. SRT L107-L111)
  - I dati non sono sensibili; nessuna informazione di conto. (Rif. SRT L63-L64)
  - Preferiti evidenziati; Recenti mantengono ultimi 10 trasferimenti. (Rif. SRT L75-L80, L255-L256)
- **Comportamenti Sistema**:
  - Caricamento rubrica da localStorage all’accesso.
  - Aggiornamento "ultimo utilizzo" su completamento trasferimento. (Rif. SRT L259-L263)
- **Messaggi Errore**: Non applicabili per caricamento rubrica; eventuali errori di lettura/scrittura localStorage gestiti con toast informativo.
- **Integrazione**: Nessuna integrazione esterna; storage locale. (Rif. SRT L55-L60)

#### 4.2 Selezione Destinatario
- **Descrizione**: Tre modalità di selezione: Preferiti, Recenti (con importo e data), Tutti i contatti (ordine alfabetico). (Rif. SRT L71-L84)
- **Flusso Utente**:
  1. Apertura sezione destinatari e scelta modalità.
  2. Facoltativa ricerca in tempo reale per nome o codice fiscale. (Rif. SRT L87-L95)
  3. Selezione contatto e conferma per procedere al trasferimento. (Rif. SRT L147-L156)
- **Elementi UI**:
  - Tabs/filtri per le tre modalità; elenco con virtual scrolling se necessario. (Rif. SRT L415-L419)
  - Mostrare per Recenti: destinatario, importo, data. (Rif. SRT L79-L80, L255-L256)
- **Regole Business**:
  - Recenti limitati agli ultimi 10 trasferimenti. (Rif. SRT L255-L256)
  - Aggiornamento automatico data "ultimo utilizzo". (Rif. SRT L259-L263)
- **Comportamenti Sistema**:
  - Al click su contatto, passaggio a schermata dettagli trasferimento con dati precompilati. (Rif. SRT L147-L156)
- **Messaggi Errore**: Nessuno specifico; eventuali errori di caricamento liste con toast.
- **Integrazione**: Dati locali; i Recenti derivano dagli esiti di esecuzione trasferimento.

#### 4.3 Ricerca Contatti
- **Descrizione**: Ricerca in tempo reale per nome o codice fiscale con debounce per non saturare risorse. (Rif. SRT L91-L96, L419-L420)
- **Flusso Utente**: Inserimento testo; risultati aggiornati dinamicamente.
- **Elementi UI**: Barra ricerca; conteggio risultati opzionale.
- **Regole Business**: Debounce 300 ms. (Rif. SRT L419-L420)
- **Comportamenti Sistema**: Filtrare localmente la rubrica; case-insensitive; CF trattato upper-case. (Rif. SRT L127-L128)
- **Messaggi Errore**: Nessuno; se nessun risultato, mostrare stato vuoto.
- **Integrazione**: Nessuna.

#### 4.4 Aggiunta Nuovo Contatto
- **Descrizione**: Creazione contatto tramite pulsante "+ Nuovo" e form con campi obbligatori/opzionali. (Rif. SRT L103-L112)
- **Flusso Utente**:
  1. Click "+ Nuovo" apre modale con form.
  2. Inserimento dati, validazione in tempo reale.
  3. Salvataggio su localStorage.
- **Elementi UI**: Modale con campi, CTA "Salva" e "Annulla".
- **Regole Business**:
  - Codice fiscale: validazione formale italiana; auto-uppercase. (Rif. SRT L119-L128)
  - IBAN (se presente): formale IT + 25 caratteri; auto-uppercase. (Rif. SRT L135-L140)
- **Comportamenti Sistema**: Bloccare salvataggio se validazioni falliscono.
- **Messaggi Errore**: Testo chiaro, es. "Il codice fiscale inserito non è valido". (Rif. SRT L395-L400)
- **Integrazione**: Nessuna.

#### 4.5 Dettagli Trasferimento
- **Descrizione**: Schermata per confermare destinatario e inserire dettagli trasferimento. (Rif. SRT L147-L156)
- **Flusso Utente**:
  1. Visualizzazione dettagli destinatario (nome, CF, IBAN se presente) con possibilità modifica IBAN. (Rif. SRT L151-L152)
  2. Inserimento importo, descrizione (max 200 char), ID riferimento (max 50 char). (Rif. SRT L175-L183)
  3. Pulsanti "Valida" (opzionale) e "Trasferisci".
- **Elementi UI**:
  - Campi: importo, descrizione con contatore caratteri, ID riferimento.
  - CTA: "Valida", "Trasferisci"; stato disabilitato con errori. (Rif. SRT L211-L212)
- **Regole Business**:
  - Importo: minimo 0,01 €, massimo 50.000 €. (Rif. SRT L163-L168)
  - Descrizione: max 200 caratteri; ID riferimento: max 50 caratteri. (Rif. SRT L175-L181)
- **Comportamenti Sistema**:
  - Validazione front-end in tempo reale su range importo e lunghezze.
  - "Trasferisci" disabilitato finché presenti errori. (Rif. SRT L211-L212)
- **Messaggi Errore**: Range importo, lunghezze superate, CF/IBAN invalidi.
- **Integrazione**: Dati verranno inviati a backend per validazione/esecuzione. (Rif. SRT L219-L224)

#### 4.6 Validazione Pre-trasferimento (API)
- **Descrizione**: Funzione opzionale per verificare correttezza dati prima dell’esecuzione. (Rif. SRT L191-L200)
- **Flusso Utente**: Click su "Valida"; mostrare spinner; eventuali errori bloccano esecuzione. (Rif. SRT L199-L207)
- **Elementi UI**: Spinner, messaggi chiari non tecnici; toast. (Rif. SRT L359-L368, L395-L400)
- **Regole Business**: Errori bloccanti impediscono attivazione di "Trasferisci". (Rif. SRT L207-L212)
- **Comportamenti Sistema**: Chiamata API di validazione; restituzione errori specifici.
- **Messaggi Errore**: Es. conto inesistente, fondi insufficienti (dal backend). (Rif. SRT L383-L387)
- **Integrazione**: Endpoint di validazione backend; timeout ragionevole (30s). (Rif. SRT L287-L288)

#### 4.7 Esecuzione Trasferimento
- **Descrizione**: Invio richiesta di trasferimento al sistema P2P di Banca Alfa tramite backend. (Rif. SRT L219-L224, L471-L480)
- **Flusso Utente**:
  1. Click "Trasferisci"; mostrare stato "Trasferimento..."; disabilitare pulsanti. (Rif. SRT L223-L228)
  2. Attesa esito; in caso di successo, navigare a schermata di conferma. (Rif. SRT L235-L244)
- **Elementi UI**: Testo dinamico bottone, stato disabilitato, spinner. (Rif. SRT L359-L364)
- **Regole Business**: Nessuna azione concorrente durante elaborazione. (Rif. SRT L227-L228)
- **Comportamenti Sistema**: Chiamata API esecuzione con gestione timeout 30s. (Rif. SRT L287-L288)
- **Messaggi Errore**: Errori di sistema (connessione, timeout) con messaggi chiari. (Rif. SRT L387-L400)
- **Integrazione**: Endpoint esecuzione backend verso Banca Alfa.

#### 4.8 Schermata di Successo
- **Descrizione**: Mostra conferma con segno di spunta e dettagli transazione. (Rif. SRT L235-L244)
- **Flusso Utente**: Visualizzazione dettagli; scelta tra "Nuovo Trasferimento" e "Torna alla Home". (Rif. SRT L243-L244)
- **Elementi UI**: Card riepilogo con transaction_id, importo, destinatario, data/ora, fees se presenti. (Rif. SRT L239-L243, L327-L335)
- **Regole Business**: Aggiornare Recenti e data ultimo utilizzo contatto. (Rif. SRT L251-L263)
- **Comportamenti Sistema**: Aggiunta a cronologia Recenti (max 10). (Rif. SRT L255-L256)
- **Messaggi Errore**: N/A in caso di successo.
- **Integrazione**: Dati da risposta esecuzione API. (Rif. SRT L307-L320)

#### 4.9 Cronologia Recenti
- **Descrizione**: Memorizzazione locale ultimi 10 trasferimenti con importo, data, destinatario. (Rif. SRT L251-L256)
- **Flusso Utente**: Accesso sezione Recenti; riutilizzo rapido trasferimenti ricorrenti. (Rif. SRT L263-L271)
- **Elementi UI**: Lista con elementi cliccabili verso flow di trasferimento.
- **Regole Business**: Mantieni solo ultimi 10; aggiorna ultimo utilizzo. (Rif. SRT L255-L263)
- **Comportamenti Sistema**: Persistenza locale in localStorage.
- **Messaggi Errore**: N/A.
- **Integrazione**: Nessuna.

#### 4.10 Gestione Errori e Feedback Utente
- **Descrizione**: Gestione a tre livelli: validazione frontend, validazione backend, errori di sistema. (Rif. SRT L375-L387)
- **Flusso Utente**: Messaggi chiari, non tecnici; colori e icone differenti; toast informativi. (Rif. SRT L395-L404, L367-L368)
- **Elementi UI**: Banner errore sul form, tooltip/label, toast, spinner. (Rif. SRT L359-L368)
- **Regole Business**: "Trasferisci" disabilitato se errori presenti. (Rif. SRT L211-L212)
- **Comportamenti Sistema**: Mostrare feedback immediato durante validazioni ed elaborazioni. (Rif. SRT L359-L364)
- **Messaggi Errore**:
  - Esempio: "Il codice fiscale inserito non è valido". (Rif. SRT L395-L400)
  - Errori backend: conto inesistente, fondi insufficienti. (Rif. SRT L383-L384)
  - Errori di sistema: connessione/timeout. (Rif. SRT L387-L388)
- **Integrazione**: Ricezione messaggi da API di validazione/esecuzione.

#### 4.11 Health Check Servizio P2P
- **Descrizione**: Monitoraggio stato servizio P2P tramite endpoint /health. (Rif. SRT L499-L507)
- **Flusso Utente**: In caso di indisponibilità, mostrare messaggio "servizio temporaneamente non disponibile" e disabilitare funzionalità. (Rif. SRT L503-L508)
- **Elementi UI**: Banner/informativo persistente finché non torna disponibile.
- **Regole Business**: Disabilitazione proattiva della funzionalità.
- **Comportamenti Sistema**: Polling o check on-demand dello stato health.
- **Messaggi Errore**: Testo non tecnico di indisponibilità servizio.
- **Integrazione**: Endpoint /health backend.

#### 4.12 Analytics e Tracciamento
- **Descrizione**: Tracciamento eventi: avvio trasferimenti, completamenti, errori; uso modalità selezione contatti. (Rif. SRT L515-L524)
- **Flusso Utente**: Nessun impatto diretto; tracking invisibile.
- **Elementi UI**: N/A.
- **Regole Business**: Distinguere Preferiti/Recenti/Ricerca; metriche di utilizzo. (Rif. SRT L523-L536)
- **Comportamenti Sistema**: Invio eventi a sistema analytics aziendale.
- **Messaggi Errore**: N/A.
- **Integrazione**: Sistema analytics interno.

#### 4.13 Accessibilità e UX
- **Descrizione**: Interfaccia moderna, responsive; card con shadow e border-radius; palette colori: blu primario, verde successo, rosso errori. (Rif. SRT L343-L352)
- **Accessibilità**: Supporto screen reader con label adeguate; contrasti WCAG 2.1 AA; navigazione via tastiera. (Rif. SRT L655-L664)
- **Stati di caricamento**: Spinner durante validazioni; pulsanti disabilitati e testo variabile. (Rif. SRT L359-L364)

### 5. SPECIFICHE TECNICHE
- **Canali coinvolti**: Canale web responsive (desktop/mobile). (Rif. SRT L343-L351)
- **Profili utente supportati**: Clienti retail; differenze di commissioni gestite lato backend (premium vs altri); il frontend visualizza la fee se presente. (Rif. SRT L327-L335)
- **Requisiti sicurezza**:
  - Traffico HTTPS end-to-end. (Rif. SRT L427-L431)
  - Sanitizzazione input per prevenire XSS. (Rif. SRT L427-L431)
  - Rotte protette da login (AuthGuard). (Rif. SRT L431-L435)
  - Nessun salvataggio di informazioni sensibili in localStorage. (Rif. SRT L433-L435)
- **Performance requirements**:
  - Debounce ricerca: 300 ms. (Rif. SRT L419-L420)
  - Virtual scrolling per rubrica con liste ampie. (Rif. SRT L415-L416)
  - Lazy loading componente P2P. (Rif. SRT L411-L415)
  - Timeout API: 30 secondi. (Rif. SRT L287-L288)
  - Tempo risposta API P2P: < 5 s nel 95% dei casi; tipicamente < 3 s. (Rif. SRT L759-L767, L491-L492)
- **Integrazioni**:
  - Backend proprietario verso API Banca Alfa (health/validate/execute). (Rif. SRT L279-L287, L471-L480)
  - Formato input: sender_tax_id, recipient_tax_id, amount, currency; opzionali description, reference_id (JSON). (Rif. SRT L295-L300)
  - Formato risposta: status, transaction_id, timestamp, sender/recipient (tax_id, account_iban), execution_date, fees. (Rif. SRT L307-L320)
- **Sincronizzazione cross-channel**: Non prevista; rubrica locale su dispositivo (localStorage). (Rif. SRT L55-L60)

### 6. TABELLE DETTAGLIO

#### 6.1 Schermata Rubrica / Selezione Destinatario

| Sezione | Elemento | Tipologia | Descrizione |
|---------|----------|-----------|-------------|
| Header | Titolo sezione | Testo | "Pagamenti P2P" |
| Toolbar | Barra ricerca | Input testo | Ricerca per nome o CF con debounce 300 ms |
| Toolbar | Pulsante "+ Nuovo" | CTA | Apre modale creazione contatto |
| Tabs | Preferiti | Tab | Elenco contatti preferiti |
| Tabs | Recenti | Tab | Ultimi 10 trasferimenti con importo e data |
| Tabs | Tutti i contatti | Tab | Elenco alfabetico completo |
| Lista | Card contatto | Card | Mostra nome, CF, eventuali email/telefono/IBAN; selezionabile |
| Stato | Empty state | Testo | Messaggio nessun risultato / nessun contatto |

#### 6.2 Modale Nuovo Contatto

| Sezione | Elemento | Tipologia | Descrizione |
|---------|----------|-----------|-------------|
| Body | Nome | Input testo | Obbligatorio |
| Body | Cognome | Input testo | Obbligatorio |
| Body | Codice Fiscale | Input testo | Obbligatorio; validazione formale; auto-uppercase |
| Body | Email | Input testo | Opzionale |
| Body | Telefono | Input testo | Opzionale |
| Body | IBAN | Input testo | Opzionale; validazione IT + 25 caratteri; auto-uppercase |
| Footer | Salva | CTA | Salva su localStorage se validazioni ok |
| Footer | Annulla | CTA | Chiude modale senza salvare |
| Errori | Messaggi campo | Testo | Testi chiari non tecnici |

#### 6.3 Schermata Dettagli Trasferimento

| Sezione | Elemento | Tipologia | Descrizione |
|---------|----------|-----------|-------------|
| Riepilogo | Destinatario | Card | Nome, CF, IBAN (modificabile) |
| Form | Importo | Input numerico | Min 0,01; Max 50.000; validazione realtime |
| Form | Valuta | Select/Testo | "EUR" |
| Form | Descrizione | Textarea | Opzionale; max 200; contatore caratteri |
| Form | ID Riferimento | Input testo | Opzionale; max 50 |
| Azioni | Valida | CTA | Chiamata validazione; mostra spinner/risultati |
| Azioni | Trasferisci | CTA | Esegue trasferimento; disabilitato con errori |
| Feedback | Spinner | Stato | Mostrato durante validazioni/trasferimento |
| Errori | Banner/Toast | Testo | Errori frontend/backend/sistema |

#### 6.4 Schermata Successo

| Sezione | Elemento | Tipologia | Descrizione |
|---------|----------|-----------|-------------|
| Hero | Icona successo | Icona | Segno di spunta |
| Body | Dettagli transazione | Card | transaction_id, importo, destinatario, data/ora, fees |
| Footer | Nuovo Trasferimento | CTA | Reinizia flusso |
| Footer | Torna alla Home | CTA | Naviga alla home |

#### 6.5 Health & Stato Servizio

| Sezione | Elemento | Tipologia | Descrizione |
|---------|----------|-----------|-------------|
| Banner | Stato servizio | Testo | "Servizio temporaneamente non disponibile"; disabilita funzioni |

#### 6.6 Toast e Messaggistica

| Sezione | Elemento | Tipologia | Descrizione |
|---------|----------|-----------|-------------|
| Toast | Info/Success/Errore | Card/toast | Messaggi chiari e non tecnici; colori coerenti |

### 7. REQUISITI DI COMPLIANCE E SICUREZZA
- **PSD2**: conformità per servizi di pagamento. (Rif. SRT L807-L815)
- **GDPR**: tutela privacy; minimizzazione dati; rubrica locale non sensibile. (Rif. SRT L811-L815, L63-L64)
- **Limiti normativi**: importi min/max obbligatori. (Rif. SRT L163-L168)
- **Audit e test**: penetration test trimestrali; audit annuale; code review continua. (Rif. SRT L823-L832)

### 8. MONITORAGGIO E KPI
- **Metriche chiave**: tempo risposta API P2P, tasso successo trasferimenti, uptime. (Rif. SRT L759-L768)
- **Tracking UX**: utilizzo Preferiti/Recenti/Ricerca. (Rif. SRT L519-L536)

### 9. NOTE E LIMITAZIONI
- La rubrica è locale (localStorage); nessuna sincronizzazione cross-device. (Rif. SRT L55-L60)
- Le commissioni sono gestite lato backend; il frontend visualizza solo il valore se presente. (Rif. SRT L327-L335)
- Il pulsante "Valida" è opzionale e non necessario per completare il trasferimento. (Rif. SRT L195-L200)

### 10. RIFERIMENTI SRT (TRACCIABILITÀ)
- Rubrica contatti e persistenza: L39-L60, L63-L67
- Selezione destinatario (Preferiti/Recenti/Tutti): L71-L84
- Ricerca real-time con debounce: L87-L96, L419-L420
- Nuovo contatto e validazioni CF/IBAN: L103-L140
- Schermata dettagli trasferimento: L147-L156, L163-L183
- Pulsanti Valida/Trasferisci e comportamenti: L191-L212, L223-L228
- Esecuzione e schermata successo: L219-L244, L239-L243
- Cronologia Recenti e ultimo utilizzo: L251-L263
- Error handling e messaggistica: L375-L404
- Performance e sicurezza: L411-L431, L759-L768, L491-L492, L287-L288
- Integrazione con Banca Alfa e API: L471-L480, L279-L320, L287-L288
- Health check: L499-L508
- Analytics: L515-L536
- Accessibilità: L655-L664
- Compliance: L807-L815, L163-L168, L823-L832

> Il presente documento utilizza esclusivamente informazioni presenti nella trascrizione SRT, organizzate e tradotte in specifiche di business implementabili.
