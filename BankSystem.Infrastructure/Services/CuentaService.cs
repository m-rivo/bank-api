using BankSystem.Core.Entities;
using BankSystem.Core.Interfaces;
using BankSystem.Core.Settings;
using BankSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Infrastructure.Services;

public class CuentaService : ICuentaService
{
    private readonly BankDbContext _context;

    public CuentaService(BankDbContext context)
    {
        _context = context;
    }

    public async Task<Cuenta> CrearCuentaAsync(int idCliente, decimal saldoInicial)
    {

        var cliente = await _context.Clientes.AnyAsync(c => c.Id == idCliente);
        if (!cliente)
        {
            throw new InvalidOperationException($"No se puede crear la cuenta: El cliente con ID {idCliente} no existe.");
        }

        string numeroGenerado;
        bool yaExiste;

        do
        {

            var random = new Random().Next(100000, 999999);
            numeroGenerado = $"CTA-{random}";


            yaExiste = await _context.Cuentas.AnyAsync(c => c.NumeroCuenta == numeroGenerado);
        } while (yaExiste);


        if (saldoInicial < ConstantesGlobales.TasaInteresMensual)
        {
            throw new InvalidOperationException($"No se puede crear la cuenta: El saldo inicial no puede ser menor a {ConstantesGlobales.TasaInteresMensual}.");
        }


        var nuevaCuenta = new Cuenta
        {
            IdCliente = idCliente,
            NumeroCuenta = numeroGenerado,
            Saldo = saldoInicial
        };

        _context.Cuentas.Add(nuevaCuenta);
        await _context.SaveChangesAsync();

        return nuevaCuenta;
    }

    public async Task<decimal> ConsultarSaldoAsync(string numeroCuenta)
    {
        var cuenta = await _context.Cuentas
            .FirstOrDefaultAsync(c => c.NumeroCuenta == numeroCuenta);

        if (cuenta == null)
            throw new KeyNotFoundException($"La cuenta {numeroCuenta} no existe.");

        return cuenta.Saldo;
    }

}