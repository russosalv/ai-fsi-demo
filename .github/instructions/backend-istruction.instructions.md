---
applyTo: 'be/*.*'
---
You are assisting with a .NET 8 Banking API project following Clean Architecture principles:

```
Banking.Solution/
├── Banking.Models/        # Entities, DTOs, DbContext
├── Banking.Logic/         # Business Logic e Servizi  
├── Banking.Infrastructure/# External Services (Banca Alfa API)
└── Banking.API/          # Web API Controllers
```

## Core Coding Rules

### 1. Dependency Injection Pattern
ALWAYS use constructor injection and register services in Program.cs:

```csharp
// Service registration
builder.Services.AddScoped<ICustomerService, CustomerService>();

// Constructor injection
public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
{
    _customerService = customerService;
    _logger = logger;
}
```

### 2. Async/Await Mandatory
ALL I/O operations MUST be asynchronous:

```csharp
// ALWAYS do this
public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
{
    var customer = await _customerService.GetCustomerByIdAsync(id);
    return Ok(customer);
}

// NEVER do this
public CustomerDto GetCustomer(int id) => _customerService.GetCustomerById(id);
```

### 3. DTO Pattern Required
Separate domain models from API contracts:

```csharp
// Entity (internal)
public class Customer { public int Id { get; set; } /* internal props */ }

// DTO (API contract)  
public class CustomerDto { public int Id { get; set; } /* public props */ }
```

## Naming Conventions

- **Classes**: `PascalCase` → `CustomerService`
- **Interfaces**: `I + PascalCase` → `ICustomerService`  
- **DTOs**: `Name + Dto` → `CustomerDto`
- **Async Methods**: `MethodNameAsync` → `GetCustomerByIdAsync`
- **Private Fields**: `_camelCase` → `_customerService`
- **Properties**: `PascalCase` → `FirstName`

## File Structure Template

For Controllers, ALWAYS follow this order:

```csharp
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    // 1. Private readonly fields
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomersController> _logger;

    // 2. Constructor with DI
    public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    // 3. Public methods ordered by importance
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers() { }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetCustomer(int id) { }
}
```

## Error Handling Pattern

ALWAYS use this try-catch structure:

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

## HTTP Status Codes
- `200 OK`: Success
- `400 Bad Request`: Validation error
- `404 Not Found`: Resource not found  
- `422 Unprocessable Entity`: Business logic error
- `500 Internal Server Error`: System error

## API Documentation
ALWAYS add XML documentation with response codes:

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

## Logging Best Practices

Use structured logging with parameters:

```csharp
// ALWAYS do this
_logger.LogInformation("Customer {CustomerId} retrieved successfully", id);
_logger.LogError(ex, "Error getting bank accounts for customer {CustomerId}", id);

// NEVER do this  
_logger.LogInformation($"Customer {id} retrieved");
```

## Entity Framework Patterns

### DbContext Configuration
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Explicit entity configuration
    modelBuilder.Entity<Customer>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.TaxCode).IsUnique();
        entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
    });
}
```

### Efficient Querying
```csharp
// Use projection for performance
var customers = await _context.Customers
    .Where(c => c.IsActive)
    .Select(c => new CustomerDto { Id = c.Id, FirstName = c.FirstName })
    .ToListAsync();

// Include only when necessary
var bankAccounts = await _context.BankAccounts
    .Include(ba => ba.Customer)
    .Where(ba => ba.CustomerId == customerId)
    .ToListAsync();
```

## Configuration Pattern

Use type-safe configuration binding:

```csharp
// Registration
builder.Services.Configure<BancaAlfaApiConfiguration>(
    builder.Configuration.GetSection("BancaAlfaApi"));

// Usage
public BancaAlfaP2PService(IOptions<BancaAlfaApiConfiguration> configuration)
{
    _configuration = configuration.Value;
}
```

## Security Rules

- NEVER hardcode API keys or credentials
- ALWAYS validate input parameters:
  ```csharp
  if (string.IsNullOrWhiteSpace(iban))
  {
      return BadRequest("IBAN cannot be empty");
  }
  ```
- Use regex for format validation (Tax Code, IBAN, etc.)
- HTTPS mandatory in production

## Mock Service Pattern

When creating mock services, follow this structure:

```csharp
public class BancaAlfaP2PMockService : IBancaAlfaP2PService
{
    private readonly ILogger<BancaAlfaP2PMockService> _logger;
    
    public BancaAlfaP2PMockService(ILogger<BancaAlfaP2PMockService> logger)
    {
        _logger = logger;
    }
    
    public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request)
    {
        _logger.LogInformation("Mock: Processing payment for amount {Amount}", request.Amount);
        
        // Simulate processing delay
        await Task.Delay(100);
        
        return new PaymentResponse { Success = true, TransactionId = Guid.NewGuid().ToString() };
    }
}
```

## Service Registration Pattern

```csharp
public static IServiceCollection AddBancaAlfaInfrastructure(
    this IServiceCollection services, 
    IConfiguration configuration,
    bool enableMocking = false)
{
    if (enableMocking || configuration.GetValue<bool>("BancaAlfaApi:EnableMocking"))
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

## Key Reminders

1. **Always use async/await for I/O operations**
2. **Always use dependency injection**
3. **Always separate DTOs from entities**  
4. **Always add proper error handling**
5. **Always use structured logging**
6. **Always validate input parameters**
7. **Always add XML documentation**
8. **Always follow the naming conventions**
9. **Never hardcode configuration values**
10. **Never use synchronous I/O operations**

When generating code, prioritize:
- Clean Architecture principles
- SOLID principles  
- Security best practices
- Performance considerations
- Maintainability and readability