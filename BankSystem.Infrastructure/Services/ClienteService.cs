using BankSystem.Core.Entities;
using BankSystem.Core.Interfaces;
using BankSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Infrastructure.Services;

public class ClienteService : IClienteService
{
    private readonly BankDbContext _context;

    // Inyectamos el DbContext a través del constructor
    public ClienteService(BankDbContext context)
    {
        _context = context;
    }

    public async Task<Cliente> CrearClienteAsync(Cliente cliente)
    {
        _context.Clientes.Add(cliente);

        await _context.SaveChangesAsync();

        return cliente;
    }

    public async Task<Cliente?> ObtenerPorIdAsync(int id)
    {
        return await _context.Clientes
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}