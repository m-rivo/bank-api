using BankSystem.Core.Entities;
using BankSystem.Core.Enums;
using BankSystem.Core.Interfaces;
using BankSystem.Core.Settings;
using BankSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Infrastructure.Services;

public class TransaccionService : ITransaccionService
{
    private readonly BankDbContext _context;

    public TransaccionService(BankDbContext context)
    {
        _context = context;
    }

    public async Task<Transaccion> RegistrarDepositoAsync(string numeroCuenta, decimal monto)
    {
        if (monto <= ConstantesGlobales.MontoMinimoDeposito) throw new ArgumentException($"El monto del depósito debe ser mayor a {ConstantesGlobales.MontoMinimoDeposito}.");

        // Usamos una transacción de base de datos para asegurar integridad
        using var dbTransaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var cuenta = await ObtenerCuentaAsync(numeroCuenta);

            // Actualizar Saldo
            cuenta.Saldo += monto;

            var transaccion = new Transaccion
            {
                IdCuenta = cuenta.Id,
                Monto = monto,
                TipoTransaccion = TipoTransaccion.D,
                Fecha = DateTime.UtcNow,
                SaldoPostTransaccion = cuenta.Saldo
            };

            _context.Transacciones.Add(transaccion);
            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();

            return transaccion;
        }
        catch
        {
            await dbTransaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Transaccion> RegistrarRetiroAsync(string numeroCuenta, decimal monto)
    {
        if (monto <= ConstantesGlobales.MontoMinimoRetiro) throw new ArgumentException($"El monto del retiro debe ser mayor a {ConstantesGlobales.MontoMinimoRetiro}.");

        using var dbTransaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var cuenta = await ObtenerCuentaAsync(numeroCuenta);

            if (cuenta.Saldo < monto)
            {
                throw new InvalidOperationException("Fondos insuficientes para realizar el retiro.");
            }

            // Actualizar Saldo
            cuenta.Saldo -= monto;

            var transaccion = new Transaccion
            {
                IdCuenta = cuenta.Id,
                Monto = monto,
                TipoTransaccion = TipoTransaccion.R,
                Fecha = DateTime.UtcNow,
                SaldoPostTransaccion = cuenta.Saldo
            };

            _context.Transacciones.Add(transaccion);
            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();

            return transaccion;
        }
        catch
        {
            await dbTransaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<Transaccion>> ObtenerHistorialAsync(string numeroCuenta)
    {
        var cuenta = await _context.Cuentas
            .FirstOrDefaultAsync(c => c.NumeroCuenta == numeroCuenta);

        if (cuenta == null) throw new KeyNotFoundException("Cuenta no encontrada.");

        return await _context.Transacciones
            .Where(t => t.IdCuenta == cuenta.Id)
            .OrderByDescending(t => t.Fecha)
            .ToListAsync();
    }

    private async Task<Cuenta> ObtenerCuentaAsync(string numeroCuenta)
    {
        var cuenta = await _context.Cuentas
            .FirstOrDefaultAsync(c => c.NumeroCuenta == numeroCuenta);

        if (cuenta == null)
            throw new KeyNotFoundException($"La cuenta con número {numeroCuenta} no existe.");

        return cuenta;
    }

    public async Task AplicarInteresAsync(Cuenta cuenta)
    {
        if (cuenta.Saldo > 0)
        {
            decimal tasaInteres = ConstantesGlobales.TasaInteresMensual;
            decimal montoInteres = cuenta.Saldo * tasaInteres;

            cuenta.Saldo += montoInteres;
            var transaccion = new Transaccion
            {
                IdCuenta = cuenta.Id,
                Monto = montoInteres,
                TipoTransaccion = TipoTransaccion.D,
                Fecha = DateTime.UtcNow,
                SaldoPostTransaccion = cuenta.Saldo
            };

            _context.Transacciones.Add(transaccion);
            await _context.SaveChangesAsync();
        }
    }
}