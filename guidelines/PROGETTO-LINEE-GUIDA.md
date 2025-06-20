# Linee Guida del Progetto Banking API

## Indice
1. [Panoramica Architetturale](#panoramica-architetturale)
2. [Best Practices di Progettazione](#best-practices-di-progettazione)
3. [Istruzioni Operative](#istruzioni-operative)
4. [Convenzioni di Codice](#convenzioni-di-codice)
5. [Gestione Errori](#gestione-errori)
6. [Sicurezza](#sicurezza)
7. [Testing e Mocking](#testing-e-mocking)
8. [Configurazione](#configurazione)
9. [Logging](#logging)
10. [Database e Entity Framework](#database-e-entity-framework)

---

## Panoramica Architetturale

### Architettura Adottata
Il progetto segue i principi della **Clean Architecture** con separazione netta delle responsabilità:

```
Banking.Solution/
├── Banking.Models/        # Entità, DTOs, DbContext
├── Banking.Logic/         # Business Logic e Servizi
├── Banking.Infrastructure/# Servizi esterni (API Banca Alfa)
└── Banking.API/          # Web API Controllers
```

### Principi Architetturali
- **Separation of Concerns**: Ogni layer ha responsabilità specifiche
- **Dependency Inversion**: I layer superiori dipendono da astrazioni (interfaces)
- **Single Responsibility**: Ogni classe ha una responsabilità ben definita
- **Open/Closed Principle**: Estensibile senza modifica del codice esistente

---

## Best Practices di Progettazione

### 1. Dependency Injection
**Principio**: Utilizzare sempre l'injection container per gestire le dipendenze

```csharp
// ✅ Corretto - Registration in Program.cs
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IBankAccountService, BankAccountService>();

// ✅ Corretto - Constructor injection
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }
}
```

### 2. Async/Await Pattern
**Principio**: Tutte le operazioni I/O devono essere asincrone

```csharp
// ✅ Corretto
public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
{
    var customer = await _customerService.GetCustomerByIdAsync(id);
    return Ok(customer);
}

// ❌ Evitare operazioni sincrone per I/O
public CustomerDto GetCustomer(int id)
{
    return _customerService.GetCustomerById(id); // Bloccante
}
```

### 3. DTO Pattern
**Principio**: Separare i modelli di dominio dai modelli di trasferimento dati

```csharp
// ✅ Entity (interno)
public class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    // ... altre proprietà interne
}

// ✅ DTO (API contract)
public class CustomerDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
}
```

### 4. Interface-Based Design
**Principio**: Definire contratti chiari attraverso interfacce

```csharp
// ✅ Interface definisce il contratto
public interface ICustomerService
{
    Task<CustomerDto?> GetCustomerByIdAsync(int customerId);
    Task<IEnumerable<BankAccountDto>> GetCustomerBankAccountsAsync(int customerId);
}

// ✅ Implementazione concreta
public class CustomerService : ICustomerService
{
    // Implementazione...
}
```

---

## Istruzioni Operative

### 1. Setup del Progetto

#### Prerequisiti
- .NET 8 SDK
- Visual Studio 2022 / VS Code
- PowerShell (Windows) o bash (Linux/macOS)

#### Avvio del Backend
```powershell
# Windows
.\start-backend.ps1

# Linux/macOS
chmod +x start-backend.sh
./start-backend.sh
```

#### Build del Progetto
```bash
# Build completo della solution
dotnet build Banking.Solution.sln

# Build specifico dell'API
dotnet build Banking.API/Banking.API.csproj
```

### 2. Configurazione Ambienti

#### Development
- Database InMemory con seed data
- Swagger UI abilitato
- CORS permissivo per frontend
- Logging dettagliato

#### Production
- HTTPS obbligatorio
- Configurazione sicura delle credenziali
- Rate limiting attivo
- Audit logging completo

### 3. Gestione Configurazioni

#### File di Configurazione
```json
// appsettings.json (base)
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "BancaAlfaApi": {
    "BaseUrl": "https://api.bancaalfa.it",
    "TimeoutSeconds": 30,
    "EnableMocking": false
  }
}

// appsettings.Development.json (override per sviluppo)
{
  "BancaAlfaApi": {
    "EnableMocking": true
  }
}
```

---

## Convenzioni di Codice

### 1. Naming Conventions

#### Classi e Interfacce
```csharp
// ✅ Classi: PascalCase
public class CustomerService { }

// ✅ Interfacce: I + PascalCase
public interface ICustomerService { }

// ✅ DTOs: Nome + Dto suffix
public class CustomerDto { }
```

#### Metodi e Proprietà
```csharp
// ✅ Metodi: PascalCase + Async suffix per metodi asincroni
public async Task<CustomerDto> GetCustomerByIdAsync(int id) { }

// ✅ Proprietà: PascalCase
public string FirstName { get; set; }

// ✅ Campi privati: _camelCase
private readonly ICustomerService _customerService;
```

### 2. Organizzazione File

#### Struttura Controller
```csharp
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    // 1. Campi privati readonly
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomersController> _logger;

    // 2. Constructor
    public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    // 3. Metodi pubblici ordinati per importanza/usage
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers() { }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetCustomer(int id) { }
}
```

### 3. Documentazione API

#### XML Documentation
```csharp
/// <summary>
/// Gets all bank accounts for a specific customer
/// </summary>
/// <param name="id">Customer ID</param>
/// <returns>List of bank accounts for the customer</returns>
/// <response code="200">Returns the customer's bank accounts</response>
/// <response code="404">Customer not found</response>
[HttpGet("{id}/bank-accounts")]
[ProducesResponseType(typeof(IEnumerable<BankAccountDto>), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<IEnumerable<BankAccountDto>>> GetCustomerBankAccounts(int id)
```

---

## Gestione Errori

### 1. Strategia di Error Handling

#### Pattern Try-Catch Standard
```csharp
public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
{
    try
    {
        var customer = await _customerService.GetCustomerByIdAsync(id);
        if (customer == null)
        {
            return NotFound($"Customer with ID {id} not found");
        }
        return Ok(customer);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting customer {CustomerId}", id);
        return StatusCode(500, "Internal server error");
    }
}
```

### 2. Codici di Stato HTTP

#### Mapping degli Status Code
- **200 OK**: Operazione completata con successo
- **400 Bad Request**: Errore di validazione input
- **404 Not Found**: Risorsa non trovata
- **422 Unprocessable Entity**: Errore di business logic
- **500 Internal Server Error**: Errore di sistema

### 3. Eccezioni Personalizzate

#### Per API Esterne
```csharp
public class BancaAlfaApiException : Exception
{
    public string ErrorCode { get; }
    public int HttpStatusCode { get; }
    
    public BancaAlfaApiException(string errorCode, int httpStatusCode, string message) 
        : base(message)
    {
        ErrorCode = errorCode;
        HttpStatusCode = httpStatusCode;
    }
}
```

---

## Sicurezza

### 1. Principi di Sicurezza

#### Gestione Credenziali
- **Mai** hardcodare API key nel codice
- Utilizzare Azure Key Vault o sistemi simili in produzione
- Rotazione regolare delle credenziali (90 giorni)

#### Validazione Input
```csharp
// ✅ Validazione parametri
if (string.IsNullOrWhiteSpace(iban))
{
    return BadRequest("IBAN cannot be empty");
}

// ✅ Validazione formato codice fiscale
private static readonly Regex TaxIdRegex = new(
    @"^[A-Z]{6}[0-9]{2}[A-Z][0-9]{2}[A-Z][0-9]{3}[A-Z]$",
    RegexOptions.Compiled);
```

### 2. HTTPS e TLS
```csharp
// ✅ HTTPS obbligatorio in produzione
if (builder.Environment.IsProduction())
{
    builder.Services.AddHttpsRedirection(options =>
    {
        options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
        options.HttpsPort = 443;
    });
}
```

### 3. CORS Configuration
```csharp
// ✅ CORS configurato per ambiente specifico
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
        else
        {
            policy.WithOrigins("https://yourdomain.com")
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
    });
});
```

---

## Testing e Mocking

### 1. Strategia di Mocking

#### Configurazione Mock Service
```json
{
  "BancaAlfaApi": {
    "EnableMocking": true,
    "MockScenarios": {
      "DefaultScenario": "success",
      "Scenarios": {
        "success": {
          "Success": true,
          "DelayMs": 0
        },
        "timeout": {
          "Success": false,
          "ErrorCode": "TIMEOUT",
          "DelayMs": 5000
        }
      }
    }
  }
}
```

#### Registrazione Servizi Mock
```csharp
public static IServiceCollection AddBancaAlfaInfrastructure(
    this IServiceCollection services, 
    IConfiguration configuration,
    bool enableMocking = false)
{
    var config = configuration.GetSection("BancaAlfaApi").Get<BancaAlfaApiConfiguration>();
    var shouldUseMock = enableMocking || (config?.EnableMocking ?? false);

    if (shouldUseMock)
    {
        services.AddScoped<IBancaAlfaP2PService, BancaAlfaP2PMockService>();
    }
    else
    {
        services.AddScoped<IBancaAlfaP2PService, BancaAlfaP2PService>();
    }
    
    return services;
}
```

---

## Configurazione

### 1. Gestione delle Configurazioni

#### Configuration Binding
```csharp
// ✅ Binding type-safe della configurazione
builder.Services.Configure<BancaAlfaApiConfiguration>(
    builder.Configuration.GetSection("BancaAlfaApi"));

// ✅ Utilizzo nel servizio
public class BancaAlfaP2PService
{
    private readonly BancaAlfaApiConfiguration _configuration;
    
    public BancaAlfaP2PService(IOptions<BancaAlfaApiConfiguration> configuration)
    {
        _configuration = configuration.Value;
    }
}
```

### 2. Environment-Specific Settings

#### File di Configurazione per Ambiente
- `appsettings.json`: Configurazione base
- `appsettings.Development.json`: Override per sviluppo
- `appsettings.Docker.json`: Override per container Docker
- `appsettings.Production.json`: Override per produzione

---

## Logging

### 1. Structured Logging

#### Configurazione Logging
```csharp
// ✅ Configurazione in appsettings.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Banking.Infrastructure": "Debug"
    }
  }
}
```

#### Best Practices per Logging
```csharp
// ✅ Structured logging con parametri
_logger.LogInformation("Customer {CustomerId} retrieved successfully", id);

// ✅ Logging errori con context
_logger.LogError(ex, "Error getting bank accounts for customer {CustomerId}", id);

// ❌ Evitare string interpolation nel logging
_logger.LogInformation($"Customer {id} retrieved"); // Non ottimale per performance
```

### 2. Log Levels

#### Utilizzo dei Livelli di Log
- **Debug**: Informazioni dettagliate per debugging
- **Information**: Eventi generali dell'applicazione
- **Warning**: Situazioni inaspettate ma gestibili
- **Error**: Errori che impediscono il completamento dell'operazione
- **Critical**: Errori che richiedono attenzione immediata

---

## Database e Entity Framework

### 1. Configurazione DbContext

#### Context Configuration
```csharp
public class BankingDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ✅ Configurazione esplicita delle entità
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TaxCode).IsUnique();
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
        });

        // ✅ Configurazione relazioni
        modelBuilder.Entity<BankAccount>(entity =>
        {
            entity.HasOne(d => d.Customer)
                  .WithMany(p => p.BankAccounts)
                  .HasForeignKey(d => d.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
```

### 2. Query Best Practices

#### Efficient Querying
```csharp
// ✅ Projection con Select per performance
var customers = await _context.Customers
    .Where(c => c.IsActive)
    .Select(c => new CustomerDto
    {
        Id = c.Id,
        FirstName = c.FirstName,
        LastName = c.LastName
    })
    .ToListAsync();

// ✅ Include solo quando necessario
var bankAccounts = await _context.BankAccounts
    .Include(ba => ba.Customer)
    .Where(ba => ba.CustomerId == customerId && ba.IsActive)
    .ToListAsync();
```

### 3. Data Seeding

#### Seed Data per Testing
```csharp
private void SeedData(ModelBuilder modelBuilder)
{
    // ✅ Seed data con ID espliciti per stabilità
    modelBuilder.Entity<Customer>().HasData(
        new Customer 
        { 
            Id = 1, 
            FirstName = "Mario", 
            LastName = "Rossi",
            TaxCode = "RSSMRA80A01H501Z",
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        }
    );
}
```

---

## Conclusioni

Queste linee guida rappresentano le best practices adottate nel progetto Banking API e devono essere seguite per garantire:

- **Consistenza**: Codice uniforme e prevedibile
- **Manutenibilità**: Facilità di modifica e estensione
- **Sicurezza**: Protezione dei dati e delle operazioni
- **Performance**: Efficienza nell'esecuzione
- **Testabilità**: Facilità di testing e debugging

### Risorse Aggiuntive

- [Clean Architecture Principles](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [ASP.NET Core Best Practices](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/best-practices)
- [Entity Framework Core Best Practices](https://docs.microsoft.com/en-us/ef/core/miscellaneous/configuring-dbcontext)
- [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)

---

*Documento aggiornato: Giugno 2025*
*Versione: 1.0*
