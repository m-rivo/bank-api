using BankSystem.Core.DTOs;
using BankSystem.Core.DTOs.Clientes;
using BankSystem.Core.Entities;
using BankSystem.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ClienteResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClienteResponse>> Crear(ClienteCreateRequest request)
    {
        // Mapeo de DTO
        var nuevoCliente = new Cliente
        {
            Nombre = request.Nombre,
            FechaNacimiento = request.FechaNacimiento,
            Sexo = request.Sexo,
            Ingresos = request.Ingresos
        };

        var creado = await _clienteService.CrearClienteAsync(nuevoCliente);

        // Response
        var response = new ClienteResponse(
            creado.Id,
            creado.Nombre,
            creado.FechaNacimiento,
            creado.Sexo,
            creado.Ingresos
        );

        return CreatedAtAction(nameof(ObtenerPorId), new { id = response.Id }, response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ClienteResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteResponse>> ObtenerPorId(int id)
    {
        var cliente = await _clienteService.ObtenerPorIdAsync(id);

        if (cliente == null)
        {
            return NotFound(new { Message = $"No se encontró el cliente con ID {id}" });
        }

        // Response
        var response = new ClienteResponse(
            cliente.Id,
            cliente.Nombre,
            cliente.FechaNacimiento,
            cliente.Sexo,
            cliente.Ingresos
        );

        return Ok(response);
    }
}