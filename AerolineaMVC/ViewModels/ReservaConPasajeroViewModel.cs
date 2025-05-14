using System.ComponentModel.DataAnnotations;

namespace AerolineaMVC.ViewModels
{
    public class ReservaConPasajeroViewModel
    {
        public int TarifaId { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Documento { get; set; }

        [Range(0, 120)]
        public int Edad { get; set; }

        [Required]
        public string ContactoEmergenciaNombre { get; set; }

        [Phone]
        public string ContactoEmergenciaTelefono { get; set; }

        [EmailAddress]
        public string ContactoEmergenciaCorreo { get; set; }
    }
}
