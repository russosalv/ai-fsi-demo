# DEMO

# 1. ICB Integrazione Core banking Fake
da un documento fake in pdf dove è descritto come utilizzare un api di core banking per eseguire pagamenti P2P
facciamo generare un servizio c# che poi verrà utilizzato da logic per effettuare il pagamento
Facciamo creare un nuovo progetto c# chiamato Bamking.Infrastructure che contiente Interfaccia e Implementazione

Obbiettivo: Dimostrare la fattibilità di poter generare codice a partire da un documento tecnico

```txt
dato il documento tecnico della banca alfa crea in /be un ulteriore progetto chiamato Banking.Infrastructure che implementi la comunicazione verso l'api descritta in documentazione.
Prima di effettuare le modifiche di codice assicurati di aver compreso il documento,
Rileggi tutto il documento, crea una strategia di implementazione e codiviti la strategia.
Il to compito e mapapre tutti i dati richiesti e le possibili eccezioni
crea interfacce e implementazioni concrete
```

# 2. Sviluppo Feature BackEnd + FrontEnd per P2P
Generazione di pagina frontend e controlle BE per poter eseguire il pagamento P2P

Obbiettivo: Dimostrare che Copilot è in grado di generare NON solo suggerimenti ma amnche sviluppi di feature complete con dominio fullstack.

```txt
Integra il service BancaAlfaP2PService all interno del progetto Bank.APi
esponi il servzio tramite controller dedicato P2P
```

```txt
adesso sviluppa la pagina frontend per utilizzare l'api appena creata,
l'utente deve poter selezionare l'elenco di persone da una rubrica interna all'app che storicizza in memoria
```

# 3. (Opzionale) Modifica Interfaccia Frontend
Si può cheidere a copilot di effettuare alcune modifiche grafiche, o di cambiare layout pagina

Obbiettivo: Dimostrare che copilot può eseguire modifiche Frontend

# 4. Sviluppo MOCK per ICB
Possibilità di mockkare il servizion ICB per poter eseguire un test completo dell applicativo
```txt
estendi il metodo AddBancaAlfaInfrastructure per poter mockare il servizio con Moq abilitabile tramite parametro true/false
se il parametro è true i vari scenario devono essere presi da appsetting.json
```

# 5. Esecuzione test E2E con Puppeteer 
Esecuzione di un test e2e per esecuzione test con puppeteer

```txt
esegui un test e2e sull applicativo

Nota:
il fe e be sono già avviati
usa dimensione schermo mobile per pupeteer

##step1
con pupeteer apri il frontend alla pagina http://localhost:4200

##step2
loggati sull applicativo con il codice fiscale del utente demo ce trovi nella schermata di login, e con una password generica (eg: 1234)

##step3
nella pagina di home clicka sul icona dei pagamenti p2p

##caso di successo
l'applicativo apre correttametne la pagina di pagamento p2p controlla l'url del sito
```

```txt
con l'esperienza che hai maturato durante questo test genera un test javascript che utilizzi pupeteer da utilizzarsi in una pipeline di Azure DevOps, lo script deve poter avviare chrome eseguire gl istessi step che hai eseguito tu nell'ultima esecuzione e rotnare 1 in caso di successo e 0 in caso di insuccesso.
genera lo script in un folder apposito chiamato e2e_fe_test nella root del progetto
```