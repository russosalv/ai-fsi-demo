# Banking FSI Demo - AI Coding Instructions

## Architecture Overview

This is a full-stack banking application demonstrating Financial Services Industry (FSI) patterns with Clean Architecture principles:

```
ai-fsi-demo/
├── be/                           # .NET 8 Backend (Clean Architecture)
│   ├── Banking.Models/           # Entities, DTOs, EF DbContext
│   ├── Banking.Logic/            # Business logic services
│   ├── Banking.Infrastructure/   # External API integrations (Banca Alfa)
│   └── Banking.API/             # Web API controllers
├── fe/                          # Angular 18 Frontend
└── e2e_fe_test/                # Puppeteer E2E tests
```

## Critical Development Patterns

### Backend (.NET 8)
**Mandatory Patterns** - Follow backend instruction file at `.github/instructions/backend-istruction.instructions.md`:
- **Async/Await**: ALL I/O operations must be asynchronous with `Async` suffix
- **Constructor DI**: Always use constructor injection, register in `Program.cs`
- **DTO Separation**: Never expose entities directly - use DTOs for API contracts
- **Try-Catch Structure**: Standard error handling with structured logging
- **XML Documentation**: Required with `ProducesResponseType` attributes

```csharp
// Standard controller pattern
public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
{
    try {
        var customer = await _customerService.GetCustomerByIdAsync(id);
        return customer == null ? NotFound($"Customer with ID {id} not found") : Ok(customer);
    }
    catch (Exception ex) {
        _logger.LogError(ex, "Error getting customer {CustomerId}", id);
        return StatusCode(500, "Internal server error");
    }
}
```

### Key Authentication Flow
Login uses Tax Code lookup: `GET /api/customers/taxcode/{taxCode}/bank-accounts`
- If returns accounts → valid user, redirect to `/home`  
- If 404 → "User doesn't exist" message

**Demo Users**:
- Mario Rossi: `RSSMRA80A01H501Z` (2 accounts)
- Giulia Bianchi: `BNCGLI85M15F205W` (1 account)

### P2P Integration Architecture
The `Banking.Infrastructure` project handles external Banca Alfa P2P API integration:
- Uses mock services in development (configurable via `EnableMocking`)
- Implements comprehensive error mapping (400/422/500 status codes)
- Validation patterns for Tax Code, IBAN, amounts (€0.01-€5000)
- Reference implementation in `P2P-README.md`

### Entity Framework Patterns
- **InMemory Database**: Used with pre-seeded demo data
- **Explicit Configuration**: Use `OnModelCreating` for entity setup
- **Projection Queries**: Always use `Select()` for DTOs to avoid over-fetching
- **Include Sparingly**: Only when actually needed for navigation properties

## Development Workflows

### Startup Commands
```powershell
# Backend only (Windows)
.\be\start-backend.ps1

# Full stack (recommended)
# Use VS Code Tasks: "Start Full Stack" or Debug config "Full Stack (Backend + Frontend)"
```

**Key URLs**:
- Backend API: `https://localhost:7086/api`
- Swagger: `https://localhost:7086/swagger`  
- Frontend: `http://localhost:4200`

### Docker Environment
Uses orchestrated containers with volume persistence and HTTPS certificates:
```bash
.\start-docker.ps1 -Build    # Full stack with containers
.\start-docker.ps1 -Down     # Stop all services
```

### Testing Strategy
- **E2E Tests**: Puppeteer tests in `/e2e_fe_test/` directory
- **Mock Strategy**: Infrastructure services mockable via `EnableMocking` config
- **Demo Flow**: Login → IBAN selection → Balance display → P2P transfer

## Project-Specific Conventions

### Configuration Patterns
Multi-environment setup with typed binding:
```csharp
// Registration
builder.Services.Configure<BancaAlfaApiConfiguration>(
    builder.Configuration.GetSection("BancaAlfaApi"));

// Usage  
public Service(IOptions<BancaAlfaApiConfiguration> config)
{
    _configuration = config.Value;
}
```

### CORS & Security
- Development: Permissive CORS for `localhost:4200`
- Production: HTTPS mandatory, restricted CORS origins
- **Never hardcode credentials** - use configuration binding

### Extension Method Pattern
Infrastructure registration follows consistent pattern:
```csharp
public static IServiceCollection AddBancaAlfaInfrastructure(
    this IServiceCollection services, 
    IConfiguration configuration,
    bool enableMocking = false)
```

## Frontend (Angular 18)

### API Configuration
Frontend uses configurable API URL in `fe/src/app/config/app.config.ts`:
```bash
cd fe && npm run config:api -- -ApiUrl "https://localhost:7086/api"
```

### Component Structure
- **Reactive Forms**: For login with Tax Code validation
- **Service Pattern**: HTTP services with RxJS observables  
- **Guard Pattern**: AuthGuard protects routes post-login
- **Responsive Design**: Mobile-first with SCSS variables

## Integration Points

### External Dependencies
- **Banca Alfa API**: P2P payment processing via `Banking.Infrastructure`
- **Entity Framework**: InMemory database with seeded demo data
- **Swagger**: Auto-generated API documentation
- **Font Awesome**: Icon library for UI components

### Cross-Component Communication
- Backend → Frontend: REST APIs with DTO contracts
- Frontend → Backend: HTTP client with typed responses  
- Mock Integration: Configurable via `appsettings` for development

## Key Files for Context
- `be/Banking.API/Program.cs`: DI configuration and service registration
- `be/Banking.Infrastructure/README.md`: P2P API integration patterns
- `guidelines/PROGETTO-LINEE-GUIDA.md`: Comprehensive coding standards
- `docker-compose.yml`: Container orchestration with HTTPS setup
- `start-*.ps1` scripts: Development workflow automation

When implementing new features, prioritize Clean Architecture separation, async patterns, and comprehensive error handling following the established conventions.