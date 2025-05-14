using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AerolineaMVC.Models
{
    public class Tarifa
    {
        [Key]
        public int Id { get; set; }  

        [Required]
        public string Tipo { get; set; } = string.Empty;  // Económica, Business, etc.

        [Required]
        public decimal Precio { get; set; }

        [ForeignKey("Vuelo")]
        public int VueloId { get; set; }
        public Vuelo? Vuelo { get; set; }

        public string Clase { get; set; }
    }
}
