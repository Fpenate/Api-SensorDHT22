using System;
using System.ComponentModel.DataAnnotations;

namespace SensorDHT22.Api.Dto
{
    public class SensorReadingDto
    {
        [Required]
        public double Temperature { get; set; } // En .NET el JSON llega en minúscula por defecto
        
        [Required]
        public double Humidity { get; set; }
        
        [Required]
        public DateTime FechaHora { get; set; }
    }
}