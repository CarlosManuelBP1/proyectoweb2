namespace AerolineaMVC.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Pasajero
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Documento { get; set; }

        [Range(0, 120)]
        public int Edad { get; set; }

        //propiedades para los contactos de Emergencia
        [Required]
        public string ContactoEmergenciaNombre { get; set; }

        [Required]
        [EmailAddress]
        public string ContactoEmergenciaCorreo { get; set; }

        [Required]
        [Phone]
        public String ContactoEmergenciaCelular { get; set; }
        //propiedades para los servicios extras
        public bool Maleta { get; set; }
        public bool Comida { get; set; }
        public bool Mascota { get; set; }

        public int ReservaId { get; set; }
        public Reserva Reserva { get; set; }


      
    }

}
