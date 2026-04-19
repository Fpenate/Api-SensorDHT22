using System;


namespace SensorDHT22.Api.Models
{
    public class SensorReading
    {
        public int Id { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public DateTime FechaHora { get; set; }
    }
}