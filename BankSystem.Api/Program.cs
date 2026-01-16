using System.Text.Json.Serialization;
using BankSystem.Core.Interfaces;
using BankSystem.Infrastructure.Persistence;
using BankSystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de Servicios (Dependency Injection)

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Esto hace que los Enums (como Sexo) se vean como texto "M"/"F" en la API
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Configuración de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración de SQLite
builder.Services.AddDbContext<BankDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registro de servicios
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ICuentaService, CuentaService>();


var app = builder.Build();

// 2. CONFIGURACIÓN DEL PIPELINE (Middleware)

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();