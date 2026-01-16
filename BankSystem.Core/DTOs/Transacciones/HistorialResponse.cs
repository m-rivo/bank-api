using BankSystem.Core.Entities;

public record HistorialResponse(
    string NumeroCuenta,
    decimal SaldoActual,
    decimal TotalDepositos,
    decimal TotalRetiros,
    IEnumerable<Transaccion> Movimientos
);