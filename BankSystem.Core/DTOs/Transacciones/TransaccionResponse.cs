namespace BankSystem.Core.DTOs;

public record TransaccionResponse(
    int Id,
    string Tipo,
    decimal Monto,
    decimal SaldoDespues,
    DateTime Fecha
);