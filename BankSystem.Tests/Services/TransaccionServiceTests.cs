using BankSystem.Core.Entities;
using BankSystem.Core.Enums;
using BankSystem.Infrastructure.Persistence;
using BankSystem.Infrastructure.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BankSystem.Tests;

public class TransaccionServiceTests
{
    private BankDbContext GetDbContext()
    {
        // Creamos una conexión SQLite en memoria
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<BankDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new BankDbContext(options);
        context.Database.EnsureCreated(); // Crea las tablas
        return context;
    }

    private async Task<Cliente> CrearClienteTest(BankDbContext context)
    {
        var cliente = new Cliente
        {
            Nombre = "Test User",
            FechaNacimiento = new DateTime(1999, 1, 1),
            Sexo = Sexo.F,
        };
        context.Clientes.Add(cliente);
        await context.SaveChangesAsync();
        return cliente;
    }

    private async Task<Cuenta> CrearCuentaTest(BankDbContext context,string numeroCuenta, int idCliente )
    {
        var cuenta = new Cuenta
        {
            Id = 1,
            NumeroCuenta = numeroCuenta,
            Saldo = 1000m,
            IdCliente = idCliente
        };
        context.Cuentas.Add(cuenta);
        await context.SaveChangesAsync();
        return cuenta;
    }


    //      INTERESES
    [Fact]
    public async Task AplicarInteresAsync_ConSaldoPositivo_DebeIncrementarSaldoYRegistrarTransaccion()
    {
        // Arrange
        var context = GetDbContext();
        var service = new TransaccionService(context);

        var cliente = await CrearClienteTest(context);
        var cuenta = await CrearCuentaTest(context, "INT-001", cliente.Id);

        // Act
        await service.AplicarInteresAsync(cuenta);

        // Assert
        // 1000 + (1000 * 0.01) = 1010
        var cuentaEnDb = await context.Cuentas.FirstAsync(c => c.Id == cuenta.Id);
        Assert.Equal(1010m, cuentaEnDb.Saldo);

        var transaccion = await context.Transacciones
            .FirstOrDefaultAsync(t => t.IdCuenta == cuenta.Id);

        Assert.NotNull(transaccion);
        Assert.Equal(10m, transaccion.Monto); // El 1% de 1000
        Assert.Equal(TipoTransaccion.D, transaccion.TipoTransaccion);
        Assert.Equal(1010m, transaccion.SaldoPostTransaccion);
    }


    //      HISTORIAL
    [Fact]
    public async Task ObtenerHistorial_DebeRetornarTodosLosMovimientosOrdenadosYSaldo()
    {
        // Arrange
        var context = GetDbContext();
        var service = new TransaccionService(context);

        var cliente = await CrearClienteTest(context);
        var cuenta = await CrearCuentaTest(context, "HST-001", cliente.Id);

        await service.RegistrarDepositoAsync("HST-001", 100m);
        await service.RegistrarDepositoAsync("HST-001", 200m);

        // Act
        var historial = await service.ObtenerHistorialAsync("HST-001");

        // Assert
        Assert.Equal(2, historial.Count());
        Assert.Equal(200m, historial.First().Monto);
    }


    //      DEPÓSITO
    [Fact]
    public async Task RegistrarDepositoAsync_DebeIncrementarSaldoYCrearTransaccion()
    {
        // Arrange
        var context = GetDbContext();
        var service = new TransaccionService(context);

        var cliente = await CrearClienteTest(context);
        var cuenta = await CrearCuentaTest(context, "DEP-001", cliente.Id);

        // Act
        await service.RegistrarDepositoAsync("DEP-001", 500m);

        // Assert
        var cuentaDb = await context.Cuentas.FirstAsync(c => c.NumeroCuenta == "DEP-001");
        Assert.Equal(1500m, cuentaDb.Saldo); // 1000 + 500

        var t = await context.Transacciones.FirstOrDefaultAsync(x => x.IdCuenta == cuenta.Id);
        Assert.NotNull(t);
        Assert.Equal(500m, t.Monto);
        Assert.Equal(TipoTransaccion.D, t.TipoTransaccion); 
    }


    //      RETIRO
    [Fact]
    public async Task RegistrarRetiroAsync_DebeDisminuirSaldoYValidarMontoMinimo()
    {
        // Arrange
        var context = GetDbContext();
        var service = new TransaccionService(context);

        var cliente = await CrearClienteTest(context);
        var cuenta = await CrearCuentaTest(context, "RET-001", cliente.Id);

        // Act
        await service.RegistrarRetiroAsync("RET-001", 200m);

        // Assert
        var cuentaDb = await context.Cuentas.FirstAsync(c => c.NumeroCuenta == "RET-001");
        Assert.Equal(800m, cuentaDb.Saldo); // 1000 - 200

        var t = await context.Transacciones.OrderByDescending(x => x.Fecha).FirstAsync();
        Assert.Equal(200m, t.Monto);
        Assert.Equal(TipoTransaccion.R, t.TipoTransaccion);
    }

}