using BankSystem.Core.Entities;

namespace BankSystem.Core.Interfaces
{
    public interface ITransaccionService
    {
        Task<Transaccion> RegistrarDepositoAsync(string numeroCuenta, decimal monto);
        Task<Transaccion> RegistrarRetiroAsync(string numeroCuenta, decimal monto);
        Task<IEnumerable<Transaccion>> ObtenerHistorialAsync(string numeroCuenta);
    }
}
