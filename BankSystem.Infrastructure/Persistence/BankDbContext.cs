using BankSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Infrastructure.Persistence
{
    public class BankDbContext : DbContext
    {
        public BankDbContext(DbContextOptions<BankDbContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<Transaccion> Transacciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // SQLite prefiere double o string para decimales
            modelBuilder.Entity<Cuenta>()
                .Property(c => c.Saldo)
                .HasConversion<double>();

            // SQLite prefiere double o string para decimales
            modelBuilder.Entity<Transaccion>()
                .Property(t => t.Monto)
                .HasConversion<double>();

            // Número de cuenta único
            modelBuilder.Entity<Cuenta>()
                .HasIndex(c => c.NumeroCuenta)
                .IsUnique();

            // Sexo "M" o "F"
            modelBuilder.Entity<Cliente>()
                .Property(c => c.Sexo)
                .HasConversion<string>();

            // Tipo de Transacción "D" o "R" (Depósito o Retiro)
            modelBuilder.Entity<Transaccion>()
                .Property(t => t.TipoTransaccion)
                .HasConversion<string>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
