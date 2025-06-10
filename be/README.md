# Banking API Solution

This is a .NET 8 banking API solution structured in three separate projects following Clean Architecture principles.

## Project Structure

```
Banking.Solution/
├── Banking.Models/     # Data models, DTOs, and Entity Framework context
├── Banking.Logic/      # Business logic and services
└── Banking.API/        # Web API controllers and configuration
```

### Banking.Models
Contains:
- **Entities**: Customer, BankAccount
- **DTOs**: CustomerDto, BankAccountDto, BalanceDto
- **Data**: BankingDbContext (Entity Framework with InMemory database)

### Banking.Logic
Contains:
- **Interfaces**: ICustomerService, IBankAccountService
- **Services**: CustomerService, BankAccountService

### Banking.API
Contains:
- **Controllers**: CustomersController, BankAccountsController
- **Configuration**: Dependency injection, CORS, Swagger

## Available APIs

### Customer APIs
- `GET /api/customers` - Get all customers
- `GET /api/customers/{id}` - Get customer by ID
- `GET /api/customers/{id}/bank-accounts` - Get all bank accounts for a customer

### Bank Account APIs
- `GET /api/bankaccounts` - Get all bank accounts
- `GET /api/bankaccounts/iban/{iban}` - Get bank account by IBAN
- `GET /api/bankaccounts/iban/{iban}/balance` - Get balance by IBAN
- `GET /api/bankaccounts/balances?ibans=iban1,iban2,iban3` - Get balances for multiple IBANs

## Requirements Fulfilled

✅ **Controller 1**: Given a customer ID, returns the customer's bank accounts
- Endpoint: `GET /api/customers/{id}/bank-accounts`

✅ **Controller 2**: Given an IBAN, returns the balance for that IBAN
- Endpoint: `GET /api/bankaccounts/iban/{iban}/balance`

✅ **Additional Features**:
- Multiple IBANs balance query
- Complete CRUD operations for customers and bank accounts
- InMemory database with Entity Framework
- Swagger documentation
- CORS support for frontend integration

## How to Run

### Option 1: Using the startup scripts (Recommended)

**Windows (PowerShell):**
```powershell
.\start-backend.ps1
```

**Linux/macOS (Bash):**
```bash
chmod +x start-backend.sh
./start-backend.sh
```

### Option 2: Manual startup

1. Navigate to the Banking.API folder:
   ```bash
   cd Banking.API
   ```

2. Run with HTTPS (recommended):
   ```bash
   dotnet run --launch-profile https
   ```

3. Or run with HTTP only:
   ```bash
   dotnet run --launch-profile http
   ```

4. Open your browser and navigate to:
   - **Swagger UI**: `https://localhost:7086/swagger` (HTTPS - recommended)
   - **API Base URL**: `https://localhost:7086/api` (HTTPS)
   - **HTTP fallback**: `http://localhost:5281/api` (auto-redirects to HTTPS)

## Sample Data

The database is seeded with sample data:

### Customers:
- **Customer 1**: Mario Rossi (ID: 1, Tax Code: RSSMRA80A01H501Z)
- **Customer 2**: Giulia Bianchi (ID: 2, Tax Code: BNCGLI85M15F205W)

### Bank Accounts:
- **IBAN**: IT60X0542811101000000123456 (Mario Rossi - Checking, Balance: €1,500.00)
- **IBAN**: IT60X0542811101000000654321 (Mario Rossi - Savings, Balance: €5,000.00)
- **IBAN**: IT60X0542811101000000789012 (Giulia Bianchi - Checking, Balance: €2,750.50)

## Example API Calls

```bash
# Get customer bank accounts
curl -X GET "https://localhost:7086/api/customers/1/bank-accounts"

# Get balance by IBAN
curl -X GET "https://localhost:7086/api/bankaccounts/iban/IT60X0542811101000000123456/balance"

# Get multiple balances
curl -X GET "https://localhost:7086/api/bankaccounts/balances?ibans=IT60X0542811101000000123456,IT60X0542811101000000654321"

# Login endpoint (for frontend authentication)
curl -X GET "https://localhost:7086/api/customers/taxcode/RSSMRA80A01H501Z/bank-accounts"
```

## Technologies Used

- **.NET 8**
- **ASP.NET Core Web API**
- **Entity Framework Core 8.0.11**
- **InMemory Database**
- **Swagger/OpenAPI**
- **Swashbuckle.AspNetCore 6.8.1** 