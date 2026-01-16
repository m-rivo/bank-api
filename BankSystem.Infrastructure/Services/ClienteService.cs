using BankSystem.Core.Entities;
using BankSystem.Core.Interfaces;
using BankSystem.Core.Settings;
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
        // Calcular edad
        var edad = DateTime.Today.Year - cliente.FechaNacimiento.Year;
        if (cliente.FechaNacimiento.Date > DateTime.Today.AddYears(-edad)) edad--;

        if (edad < 18 || edad > 99)
            throw new InvalidOperationException("El cliente debe tener entre 18 y 99 años.");
        

        if (cliente.Ingresos < ConstantesGlobales.IngresosMinimos)
            throw new InvalidOperationException($"El cliente debe tener ingresos mínimos de {ConstantesGlobales.IngresosMinimos}.");

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