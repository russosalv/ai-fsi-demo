using Microsoft.EntityFrameworkCore;
using Banking.Models.Data;
using Banking.Logic.Interfaces;
using Banking.Logic.Services;
using Banking.Infrastructure.Interfaces;
using Banking.Infrastructure.Services;
using Banking.Infrastructure.Configuration;
using Banking.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Entity Framework with InMemory database
builder.Services.AddDbContext<BankingDbContext>(options =>
    options.UseInMemoryDatabase("BankingDb"));

// Add business logic services
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IBankAccountService, BankAccountService>();

// Add Banca Alfa Infrastructure services
builder.Services.AddBancaAlfaInfrastructure(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure HTTPS (solo in produzione)
if (builder.Environment.IsProduction())
{
    builder.Services.AddHttpsRedirection(options =>
    {
        options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
        options.HttpsPort = 443; // Porta interna del container
    });
}

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Initialize database with seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BankingDbContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
// Abilita Swagger anche in produzione per i container Docker
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Banking API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("AllowFrontend");

// Use HTTPS redirection only in production and only if HTTPS is available
if (app.Environment.IsProduction() && app.Configuration["ASPNETCORE_URLS"]?.Contains("https") == true)
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();
