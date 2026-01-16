using BankSystem.Core.Interfaces;
using BankSystem.Core.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CuentasController : ControllerBase
{
    private readonly ICuentaService _cuentaService;

    public CuentasController(ICuentaService cuentaService)
    {
        _cuentaService = cuentaService;
    }


    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Crear([FromBody] CuentaCreateRequest request)
    {
        try
        {
            var cuenta = await _cuentaService.CrearCuentaAsync(request.IdCliente, request.SaldoInicial);

            return CreatedAtAction(
                nameof(ConsultarSaldo),
                new { numeroCuenta = cuenta.NumeroCuenta },
                cuenta
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }


    [HttpGet("{numeroCuenta}/saldo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConsultarSaldo(string numeroCuenta)
    {
        try
        {
            var saldo = await _cuentaService.ConsultarSaldoAsync(numeroCuenta);
            return Ok(new
            {
                NumeroCuenta = numeroCuenta,
                SaldoActual = saldo,
                FechaConsulta = DateTime.UtcNow
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Message = $"La cuenta {numeroCuenta} no existe en nuestro sistema." });
        }
    }
}