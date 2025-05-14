using System.ComponentModel.DataAnnotations;

namespace AerolineaMVC.Models.ViewModels
{
    public class ReservaViewModel
    {
        public int TarifaId { get; set; }

        // Cliente
        [Required]
        public string NombreCliente { get; set; }

        [Required, EmailAddress]
        public string CorreoCliente { get; set; }

        public int VueloId { get; set; }

        public Tarifa Tarifa { get; set; }
        public Vuelo Vuelo { get; set; }

        // Lista de pasajeros (incluye contacto de emergencia por cada pasajero)
        public List<PasajeroViewModel> Pasajeros { get; set; } = new List<PasajeroViewModel>();

    }

   
}
