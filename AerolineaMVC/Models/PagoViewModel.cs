namespace AerolineaMVC.Models
{
    public class PagoViewModel
    {
        public string MetodoPago { get; set; } // "Tarjeta" o "PayPal"

        // Solo si es "Tarjeta"
        public string TipoTarjeta { get; set; } // "Visa" o "MasterCard"
        public string NumeroTarjeta { get; set; }
        public string FechaExpiracion { get; set; }
        public string CVV { get; set; }

        public bool PagoExitoso { get; set; }
        public string PNR { get; set; }
        public string QrCodeBase64 { get; set; }
        public string CorreoPayPal { get; set; }
        public decimal Monto { get; set; } // Precio total a pagar


    }

}
