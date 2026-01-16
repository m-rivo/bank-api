namespace BankSystem.Core.DTOs;

public record SaldoResponse(
    string NumeroCuenta,
    decimal SaldoActual
);