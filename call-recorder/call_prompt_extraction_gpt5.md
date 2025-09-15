Analizza il file SRT della call tra Business Manager e Business Analyst e crea una Business Specification completa seguendo questa struttura precisa.

**INPUT RICHIESTO:**
- File SRT contenente trascrizione della call sui requisiti P2P

**OUTPUT RICHIESTO:**
- Documento Business Specification markdown formattato

## STRUTTURA DOCUMENTO DA CREARE

### 1. HEADER
```markdown
# [Nome Progetto derivato dalla call]

[Nome Banca/Organizzazione]
[Direzione responsabile]

| Metadato | Valore |
|----------|--------|
| Nome file | [Nome progetto]_BS |
| Classificazione | Business Specification |
| Autore | [Derivato dalla call] |
| Versione | 1.0 |
| Data creazione | [Data attuale] |
| Stato | Draft |
```

### 2. CHANGE LOG
Tabella con versioning del documento

### 3. INTRODUZIONE
- **Obiettivo**: Scopo del progetto basato sulla call
- **Contesto**: 
  - Banche coinvolte
  - Target clienti 
  - Canali impattati
  - Vantaggi attesi

### 4. REQUISITI FUNZIONALI (CORE)
Per ogni funzionalità discussa nella call:

#### 4.X [Nome Funzionalità]
- **Descrizione**: Cosa deve fare
- **Flusso Utente**: Step-by-step process
- **Elementi UI**: Componenti interfaccia
- **Regole Business**: Validazioni e logiche
- **Comportamenti Sistema**: Azioni/reazioni
- **Messaggi Errore**: Casistiche e testi
- **Integrazione**: Altri sistemi/canali

### 5. SPECIFICHE TECNICHE
- Canali coinvolti
- Profili utente supportati
- Requisiti sicurezza (SCA, etc.)
- Performance requirements
- Sincronizzazione cross-channel

### 5. TABELLE DETTAGLIO
Per ogni schermata:

| Sezione | Elemento | Tipologia | Descrizione |
|---------|----------|-----------|-------------|
| [Area] | [Componente] | [CTA/Testo/Card] | [Comportamento] |

## ISTRUZIONI ANALISI SRT

### ESTRAZIONE REQUISITI
1. Identifica ogni funzionalità menzionata dal Business Manager
2. Trasforma richieste business in specifiche implementabili
3. Cattura tutte le regole di validazione
4. Documenta eccezioni e casi limite
5. Estrai requisiti di sicurezza e compliance

### TRADUZIONE BUSINESS-TO-TECHNICAL
- Converti linguaggio colloquiale in specifiche precise
- Definisci comportamenti sistema per ogni user story
- Specifica integrazioni necessarie
- Identifica impatti su architettura esistente

### ORGANIZZAZIONE CONTENUTI
- Struttura gerarchica con numerazione (2.1, 2.1.1)
- Terminologia coerente
- Riferimenti incrociati tra sezioni
- Raggruppamento logico per area funzionale

## REGOLE OUTPUT

### FORMATO
- Linguaggio italiano professionale
- Stile formale, preciso, non ambiguo
- Markdown ben strutturato
- Tabelle formattate correttamente

### COMPLETEZZA
Il documento deve:
- Coprire tutti i punti discussi nell'SRT
- Essere implementabile da un team dev
- Includere sufficienti dettagli per evitare ambiguità
- Mantenere tracciabilità con requisiti originali
- Garantire la massima accuratezza nell'estrazione e nella rappresentazione dei requisiti e delle specifiche.
- Utilizzare esclusivamente i dati e le informazioni contenuti nell'SRT, senza aggiungere o inventare elementi non presenti nella trascrizione.

### ESCLUSIONI
NON includere:
- Dettagli implementativi del codice
- Tecnologie specifiche (salvo richiesta esplicita)
- Informazioni non discusse nella call

### VALIDAZIONE QUALITÀ
Assicurati che il documento:
- Sia actionable per developers
- Rispetti standard documentali enterprise
- Mantenga coerenza interna
- Includa tutti gli aspetti di UX, security, integration
- Garantisce accuratezza nella rappresentazione delle informazioni.

Procedi con l'analisi dell'SRT e genera la Business Specification completa.