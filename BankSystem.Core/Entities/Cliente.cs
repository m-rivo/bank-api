using BankSystem.Core.Enums;
namespace BankSystem.Core.Entities
{
    
    public class Cliente
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public Sexo Sexo { get; set; }
        public decimal Ingresos { get; set; }

        public ICollection<Cuenta> Cuentas { get; set; } = new List<Cuenta>();
    }
}
