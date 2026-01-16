namespace BankSystem.Core.Entities
{
    public class Cuenta
    {
        public int Id { get; set; }
        public string NumeroCuenta { get; set; } = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        public decimal Saldo { get; set; }
        public int IdCliente { get; set; }
        public Cliente Cliente { get; set; } = null!;

        public ICollection<Transaccion> Transacciones { get; set; } = new List<Transaccion>();
    }
}
