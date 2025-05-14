using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AerolineaMVC.Models
{
    public class Reserva
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string NombreCliente { get; set; }

        [Required]
        [EmailAddress]
        public string CorreoCliente { get; set; }

        [Required]
        public DateTime FechaReserva { get; set; } = DateTime.Now;


        [ForeignKey("Tarifa")]
        public int TarifaId { get; set; }
        public Tarifa Tarifa { get; set; }

        public ICollection<Pasajero> Pasajeros { get; set; }
    }
}
