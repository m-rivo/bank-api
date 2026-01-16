using BankSystem.Core.Interfaces;
using BankSystem.Core.Entities;
using BankSystem.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using BankSystem.Core.Enums;

namespace BankSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransaccionesController : ControllerBase
{
    private readonly ITransaccionService _transaccionService;

    public TransaccionesController(ITransaccionService transaccionService)
    {
        _transaccionService = transaccionService;
    }

    /// <summary>
    /// Registra un depósito en una cuenta específica.
    /// </summary>
    [HttpPost("deposito")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Transaccion))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deposito([FromBody] TransaccionRequest request)
    {
        try
        {
            var resultado = await _transaccionService.RegistrarDepositoAsync(request.NumeroCuenta, request.Monto);
            return CreatedAtAction(nameof(GetHistorial), new { numeroCuenta = request.NumeroCuenta }, resultado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Registra un retiro de una cuenta (Monto mínimo > 100).
    /// </summary>
    [HttpPost("retiro")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Transaccion))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Retiro([FromBody] TransaccionRequest request)
    {
        try
        {
            var resultado = await _transaccionService.RegistrarRetiroAsync(request.NumeroCuenta, request.Monto);
            return CreatedAtAction(nameof(GetHistorial), new { numeroCuenta = request.NumeroCuenta }, resultado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (InvalidOperationException ex) // Saldo insuficiente
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (ArgumentException ex) // Monto <= 100
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene el resumen de cuenta y todos los movimientos.
    /// </summary>
    [HttpGet("historial/{numeroCuenta}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HistorialResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHistorial(string numeroCuenta)
    {
        try
        {
            var movimientos = await _transaccionService.ObtenerHistorialAsync(numeroCuenta);

            // Cálculos
            var listaMovimientos = movimientos.ToList();
            var saldoActual = listaMovimientos.FirstOrDefault()?.SaldoPostTransaccion ?? 0;
            var totalDepositos = listaMovimientos.Where(t => t.TipoTransaccion == TipoTransaccion.D).Sum(t => t.Monto);
            var totalRetiros = listaMovimientos.Where(t => t.TipoTransaccion == TipoTransaccion.R).Sum(t => t.Monto);

            var response = new HistorialResponse(
                numeroCuenta,
                saldoActual,
                totalDepositos,
                totalRetiros,
                listaMovimientos
            );

            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }
}