using QRCoder;
using System.ComponentModel.DataAnnotations;

namespace AerolineaMVC.Models.ViewModels
{
    public class ReservaViewModel
    {
        public int TarifaId { get; set; }

        [Required]
        public string NombreCliente { get; set; }

        [Required, EmailAddress]
        public string CorreoCliente { get; set; }

        public int VueloId { get; set; }

        public Tarifa Tarifa { get; set; }
        public Vuelo Vuelo { get; set; }

        public decimal PrecioTarifa { get; set; }

        public List<PasajeroViewModel> Pasajeros { get; set; } = new List<PasajeroViewModel>();

        public string QrCodeBase64 { get; set; }
        public PayloadGenerator.Payload PNR { get; set; }

        public string? Asiento { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal TotalFinal { get; set; }
        public string TextoDescuento { get; set; }


        public decimal CalcularTotal()
        {
            decimal total = 0;

            
            if (Tarifa != null)
            {
              
                total += Tarifa.Precio * Pasajeros.Count;
            }

            
            foreach (var p in Pasajeros)
            {
                if (p.Maleta) total += 150000;
                if (p.Comida) total += 100000;
                if (p.Mascota) total += 200000;
            }

            return total;
        }


    }
}

