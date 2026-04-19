using System;
using System.Collections.Generic;

namespace SensorDHT22.Api.Dto
{
    public class Esp32StatusResponseDto
    {
        // Propiedad para saber si está vivo
        public string Estado { get; set; } = "Offline";
        
        // La lectura más reciente
        public SensorReadingDto? UltimaLectura { get; set; }
        
        // El historial de lecturas (como el de los carros)
        public List<SensorReadingDto> LecturasRecientes { get; set; } = new List<SensorReadingDto>();
    }
}