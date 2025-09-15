# Guida rapida: Configurazione MSSQL MCP Server con Docker (.NET)

**Basato su:**  
[aadversteeg/mssqlclient-mcp-server](https://github.com/aadversteeg/mssqlclient-mcp-server)

## 1. Prerequisiti

- Docker installato
- Accesso a una istanza SQL Server
- Claude Desktop configurato per MCP

## 2. Modalità di funzionamento

Il server opera in due modalità automaticamente rilevate dalla connection string:

- **Database Mode**: Connection string con database specifico (`Database=NomeDB`)
- **Server Mode**: Connection string senza database specifico (operazioni multi-database)

## 3. Avvio Container Docker

### Database Mode (database specifico)
```bash
docker run --rm -i \
  -e "MSSQL_CONNECTIONSTRING=Server=your_server;Database=your_db;User Id=your_user;Password=your_password;TrustServerCertificate=True;" \
  -e "DatabaseConfiguration__EnableExecuteQuery=true" \
  -e "DatabaseConfiguration__EnableExecuteStoredProcedure=true" \
  -e "DatabaseConfiguration__EnableStartQuery=true" \
  -e "DatabaseConfiguration__EnableStartStoredProcedure=true" \
  aadversteeg/mssqlclient-mcp-server:latest
```

### Server Mode (multi-database)
```bash
docker run --rm -i \
  -e "MSSQL_CONNECTIONSTRING=Server=your_server;User Id=your_user;Password=your_password;TrustServerCertificate=True;" \
  -e "DatabaseConfiguration__EnableExecuteQuery=true" \
  -e "DatabaseConfiguration__EnableExecuteStoredProcedure=true" \
  -e "DatabaseConfiguration__EnableStartQuery=true" \
  -e "DatabaseConfiguration__EnableStartStoredProcedure=true" \
  aadversteeg/mssqlclient-mcp-server:latest
```

## 4. Configurazione Claude Desktop

Aggiungere nel file di configurazione Claude Desktop (`mcpServers`):

```json
{
  "servers": {
    "mssql-docker": {
      "command": "docker",
      "args": [
        "run",
        "--rm",
        "-i",
        "-e", "MSSQL_CONNECTIONSTRING=Server=your_server;Database=your_db;User Id=your_user;Password=your_password;TrustServerCertificate=True;",
        "-e", "DatabaseConfiguration__EnableExecuteQuery=true",
        "-e", "DatabaseConfiguration__EnableExecuteStoredProcedure=true",
        "-e", "DatabaseConfiguration__EnableStartQuery=true",
        "-e", "DatabaseConfiguration__EnableStartStoredProcedure=true",
        "aadversteeg/mssqlclient-mcp-server:latest"
      ]
    }
  }
}
```

## 5. Variabili di ambiente per configurazione

| Variabile | Descrizione | Default |
|-----------|-------------|---------|
| `MSSQL_CONNECTIONSTRING` | Connection string SQL Server | **Richiesta** |
| `DatabaseConfiguration__EnableExecuteQuery` | Abilita esecuzione query | `false` |
| `DatabaseConfiguration__EnableExecuteStoredProcedure` | Abilita stored procedures | `false` |
| `DatabaseConfiguration__EnableStartQuery` | Abilita query in background | `false` |
| `DatabaseConfiguration__EnableStartStoredProcedure` | Abilita SP in background | `false` |
| `DatabaseConfiguration__DefaultCommandTimeoutSeconds` | Timeout comandi (1-3600s) | `30` |

## 6. Funzionalità principali

- **Query SQL**: Esecuzione diretta e in background
- **Stored Procedures**: Discovery parametri e esecuzione con type-safe conversion
- **Schema Discovery**: Lista tabelle, schema dettagliati, metadata
- **Session Management**: Operazioni long-running con monitoraggio
- **JSON Schema Output**: Parametri compatibili per validazione
- **Timeout Management**: Configurabile globalmente e per operazione

## 7. Esempi di connection string

```bash
# Database Mode con autenticazione SQL
Server=database.example.com;Database=Northwind;User Id=sa;Password=YourPassword;TrustServerCertificate=True;

# Server Mode con porta specifica
Server=database.example.com,1433;User Id=sa;Password=YourPassword;TrustServerCertificate=True;

# Database Mode con Windows Authentication (non funziona in container)
Server=database.example.com;Database=Northwind;Integrated Security=SSPI;TrustServerCertificate=True;
```

## 8. Debug e monitoraggio

- Controllare i log del container: `docker logs <container_id>`
- Usare `get_command_timeout` per verificare configurazione timeout
- Monitorare sessioni background con `get_session_status`

***

**Per dettagli completi e documentazione API:**  
👉 [https://github.com/aadversteeg/mssqlclient-mcp-server](https://github.com/aadversteeg/mssqlclient-mcp-server)
