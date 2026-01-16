using BankSystem.Core.Entities;

namespace BankSystem.Core.Interfaces
{
    public interface IClienteService
    {
        Task<Cliente> CrearClienteAsync(Cliente cliente);
        Task<Cliente?> ObtenerPorIdAsync(int id);
    }
}