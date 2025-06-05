namespace AerolineaMVC.Models.ViewModels

{
    public class PasajeroViewModel
    {
        public string Nombre { get; set; }
        public string Documento { get; set; }
        public int Edad { get; set; }
        public string CorreoCliente { get; set; }
        public string ContactoEmergenciaNombre { get; set; }
        public string ContactoEmergenciaCelular { get; set; }
        public string ContactoEmergenciaCorreo { get; set; }
        public string? Asiento { get; set; }

        public bool Maleta { get; set; }
        public bool Comida { get; set; }
        public bool Mascota { get; set; }

    }
}
