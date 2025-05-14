using System;
using System.ComponentModel.DataAnnotations;

namespace AerolineaMVC.Models
{
    public class Vuelo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Origen { get; set; } = string.Empty;

        [Required]
        public string Destino { get; set; } = string.Empty;

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public bool Disponibilidad { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad de asientos debe ser al menos 1.")]
        public int CantidadAsientos { get; set; } // Total de asientos en el avión

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "El número de asientos disponibles no puede ser negativo.")]
        public int AsientosDisponibles { get; set; } 

        public ICollection<Tarifa>? Tarifas { get; set; }


        // Constructor para inicializar los asientos disponibles al crear un vuelo
        public Vuelo()
        {
            AsientosDisponibles = CantidadAsientos;
        }

        // Método para reservar un asiento
        public bool ReservarAsiento()
        {
            if (AsientosDisponibles > 0)
            {
                AsientosDisponibles--;
                return true;
            }
            return false;
        }

        // Método para verificar disponibilidad
        public bool HayDisponibilidad()
        {
            return AsientosDisponibles > 0;
        }
    }
}
