namespace BankSystem.Core.DTOs;

public record CuentaCreateRequest(
    int IdCliente,
    decimal SaldoInicial
);