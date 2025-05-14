using Microsoft.EntityFrameworkCore;

using AerolineaMVC.Models;
namespace AerolineaMVC.Data
{
    public class AerolineaContext : DbContext
    {
        // Constructor para pasar las opciones de contexto
        public AerolineaContext(DbContextOptions<AerolineaContext> options)
            : base(options)
        {
        }

        // DbSet para las tablas
        public DbSet<Vuelo> Vuelos { get; set; }
        public DbSet<Tarifa> Tarifas { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Pasajero> Pasajeros { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la propiedad 'Precio' en 'Tarifa'
            modelBuilder.Entity<Tarifa>()
                .Property(t => t.Precio)
                .HasPrecision(18, 2);
        }
    }
}
