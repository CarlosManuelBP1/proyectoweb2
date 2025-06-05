using global::AerolineaMVC.Models.ViewModels;
using System.Collections.Generic;

namespace AerolineaMVC.ViewModels
{
    public class ReservaTempModel
    {
        public int TarifaId { get; set; }
        public List<PasajeroViewModel> Pasajeros { get; set; }
    }
}
