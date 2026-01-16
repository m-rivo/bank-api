using BankSystem.Core.Enums;
namespace BankSystem.Core.DTOs.Clientes;

public record ClienteResponse(
    int Id,
    string Nombre,
    DateTime FechaNacimiento,
    Sexo Sexo,
    decimal Ingresos
);