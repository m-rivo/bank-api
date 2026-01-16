namespace BankSystem.Core.DTOs;
public record TransaccionRequest(
    string NumeroCuenta,
    decimal Monto
);