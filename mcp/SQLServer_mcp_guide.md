# Guida rapida: Configurazione MSSQL MCP Server (Preview)

**Basato su:**  
[Introducing MSSQL MCP Server (Preview) - Azure SQL Devs’ Corner](https://devblogs.microsoft.com/azure-sql/introducing-mssql-mcp-server/)

## 1. Prerequisiti

- Node.js installato
- Accesso a una istanza SQL Server
- Git installato (opzionale per clonare il repo)

## 2. Scarica ed installa MCP Server

```bash
git clone https://github.com/Azure-Samples/azure-sql-mcpserver-node
cd azure-sql-mcpserver-node
npm install
npm run build
```

Dopo la `build` troverai il file `index.js` nella cartella `dist`.

## 3. Configura mcp.json

Esempio di entry:

```json
{
  "servers": {
    "MSSQL MCP": {
      "type": "stdio",
      "command": "node",
      "args": ["C:\\percorso\\completo\\dist\\index.js"],
      "env": {
        "SERVER_NAME": "<nome-server>",
        "DATABASE_NAME": "<nome-db>",
        "READONLY": "false"
      }
    }
  }
}
```

**Nota**: sostituisci `C:\\percorso\\completo\\dist\\index.js` con il path reale.

## 4. Avvio & Debug

- Avvia MCP server o utilizza l’integrazione con Visual Studio Code Azure SQL Copilot/Claude.
- Per debug, controlla i log Node o aggiungi variabili di ambiente specifiche per tracing.

***

**Per dettagli completi:** vedi articolo Microsoft  
👉 [https://devblogs.microsoft.com/azure-sql/introducing-mssql-mcp-server/](https://devblogs.microsoft.com/azure-sql/introducing-mssql-mcp-server/)

[1](https://devblogs.microsoft.com/azure-sql/introducing-mssql-mcp-server/)