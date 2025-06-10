<!-- Test dei Toast Messages -->

Ora i messaggi di errore e validazione appariranno come toast eleganti invece che come alert. 

### Caratteristiche dei Toast implementati:

✅ **Design Moderno**: Gradienti colorati, ombre e animazioni fluide
✅ **Tipologie Multiple**: Success (verde), Error (rosso), Warning (arancione), Info (blu)  
✅ **Auto-dismiss**: Si chiudono automaticamente dopo 5-10 secondi
✅ **Dismissible**: Cliccabile per chiudere manualmente
✅ **Stacking**: Multipli toast si impilano ordinatamente
✅ **Responsive**: Si adattano al mobile
✅ **Progress Bar**: Barra di avanzamento per la durata
✅ **Messaggi Multi-linea**: Supporto per errori di validazione multipli
✅ **Posizionamento**: Top-right fisso con z-index alto

### Esempi di utilizzo nel P2P Transfer:

1. **Validazione Errori**: Mostra lista numerata di errori
2. **Validazione Successo**: Conferma che i dati sono corretti  
3. **Trasferimento Completato**: Dettagli del trasferimento inviato
4. **Errori Specifici**: Messaggi personalizzati per ogni tipo di errore API
5. **Contatto Aggiunto**: Conferma aggiunta nuovo contatto
6. **Contatto Duplicato**: Warning per codici fiscali esistenti

### Integrazione:

- ToastService iniettato nel P2PTransferComponent
- Toast component aggiunto in AppComponent (globale)
- Sostituiti tutti gli alert() con toast appropriati
- Messaggi di errore più informativi e user-friendly
