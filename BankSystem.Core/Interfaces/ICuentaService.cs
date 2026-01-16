using BankSystem.Core.Entities;

namespace BankSystem.Core.Interfaces
{
    public interface ICuentaService
    {
        Task<Cuenta> CrearCuentaAsync(int idCliente, decimal saldoInicial);
        Task<decimal> ConsultarSaldoAsync(string numeroCuenta);
    }
}
