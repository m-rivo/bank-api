namespace BankSystem.Core.Entities
{
    public enum TipoTransaccion 
    {
        D, // Depósito
        R // Retiro
    } 

    public class Transaccion
    {
        public int Id { get; set; }
        public int IdCuenta { get; set; }
        public TipoTransaccion TipoTransaccion { get; set; }
        public decimal Monto { get; set; }
        public decimal SaldoPostTransaccion { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
    }
}
