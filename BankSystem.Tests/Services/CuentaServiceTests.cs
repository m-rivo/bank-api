using BankSystem.Core.Entities;
using BankSystem.Core.Enums;
using BankSystem.Infrastructure.Persistence;
using BankSystem.Infrastructure.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Tests.Services
{
    public class CuentaServiceTests
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


        //      CREAR CUENTA
        [Fact]
        public async Task CrearCuentaAsync_DebeGenerarNumeroDeCuentaYSaldoInicial()
        {
            // Arrange
            var context = GetDbContext();
            var service = new CuentaService(context);

            var cliente = await CrearClienteTest(context);

            // Act
            var nuevaCuenta = await service.CrearCuentaAsync(cliente.Id, 1000m);

            // Assert
            Assert.NotNull(nuevaCuenta.NumeroCuenta);
            Assert.StartsWith("CTA-", nuevaCuenta.NumeroCuenta);
            Assert.Equal(1000m, nuevaCuenta.Saldo);
            Assert.Equal(cliente.Id, nuevaCuenta.IdCliente);
        }


        //      CONSULTAR SALDO
        [Fact]
        public async Task ConsultarSaldoAsync_CuandoCuentaExiste_DebeRetornarSaldoCorrecto()
        {
            // Arrange
            var context = GetDbContext();
            var service = new CuentaService(context);

            var cliente = await CrearClienteTest(context);
            decimal saldoEsperado = 1500m;
            var numeroCuenta = "SALDO-001";

            var cuenta = new Cuenta
            {
                NumeroCuenta = numeroCuenta,
                Saldo = saldoEsperado,
                IdCliente = cliente.Id
            };
            context.Cuentas.Add(cuenta);
            await context.SaveChangesAsync();

            // Act
            decimal saldoObtenido = await service.ConsultarSaldoAsync(numeroCuenta);

            // Assert
            Assert.Equal(saldoEsperado, saldoObtenido);
        }


        //      EXCEPCIÓN CONSULTAR SALDO
        [Fact]
        public async Task ConsultarSaldoAsync_CuandoCuentaNoExiste_DebeLanzarKeyNotFoundException()
        {
            // Arrange
            var context = GetDbContext();
            var service = new CuentaService(context);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                service.ConsultarSaldoAsync("CUENTA-INEXISTENTE")
            );

            Assert.Contains("no existe", exception.Message);
        }
    }
}
