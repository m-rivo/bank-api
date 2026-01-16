using BankSystem.Core.Enums;
namespace BankSystem.Core.Entities
{

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
