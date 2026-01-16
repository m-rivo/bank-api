using BankSystem.Core.Enums;
namespace BankSystem.Core.DTOs;

public record ClienteCreateRequest(
    string Nombre,
    DateTime FechaNacimiento,
    Sexo Sexo,
    decimal Ingresos
);