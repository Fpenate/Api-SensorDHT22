using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SensorDHT22.Api.Data;
using SensorDHT22.Api.Dto;
using SensorDHT22.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SensorDHT22.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorReadingsController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public SensorReadingsController(AppDbContext context)
        {
            _context = context;
        }

        // Mapeo de DTO a Entidad
        private SensorReading ToEntity(SensorReadingDto dto) => new SensorReading
        {
            Temperature = dto.Temperature,
            Humidity = dto.Humidity,
            FechaHora = dto.FechaHora
        };

        // Mapeo de Entidad a DTO
        private SensorReadingDto ToDto(SensorReading entity) => new SensorReadingDto
        {
            Temperature = entity.Temperature,
            Humidity = entity.Humidity,
            FechaHora = entity.FechaHora
        };

        // POST: api/sensorreadings
        [HttpPost]
        public async Task<IActionResult> CreateSensorReading(SensorReadingDto sensorDto)
        {
            // Creamos la entidad directamente aquí
            var reading = new SensorReading
            {
                Temperature = sensorDto.Temperature,
                Humidity = sensorDto.Humidity,
                
                // ¡EL CAMBIO MÁGICO!
                // Ignoramos sensorDto.FechaHora y usamos la hora EXACTA del servidor de Somee
                FechaHora = DateTime.Now 
            };

            _context.SensorReadings.Add(reading);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(CreateSensorReading), new { id = reading.Id }, sensorDto);
        }

        // GET: api/sensorreadings (EL NUEVO)
        [HttpGet]
        public async Task<ActionResult<Esp32StatusResponseDto>> GetSensorStatus()
        {
            var response = new Esp32StatusResponseDto();

            // 1. Obtener la última lectura registrada
            var ultimaLectura = await _context.SensorReadings
                .OrderByDescending(r => r.FechaHora)
                .FirstOrDefaultAsync();

            if (ultimaLectura != null)
            {
                response.UltimaLectura = ToDto(ultimaLectura);

                // 2. LÓGICA PARA SABER SI ESTÁ ONLINE U OFFLINE
                // Restamos la hora actual de la hora del último dato
                TimeSpan diferencia = DateTime.Now - ultimaLectura.FechaHora;
                
                // Si la diferencia es menor a 1 minuto, está Online
                if (diferencia.TotalMinutes <= 1)
                {
                    response.Estado = "Online";
                }
                else
                {
                    response.Estado = "Offline";
                }
            }

            // 3. Obtener las últimas 20 lecturas para el historial (como la lista de carros)
            var lecturasRecientes = await _context.SensorReadings
                .OrderByDescending(r => r.FechaHora)
                .Take(20) // Trae solo los últimos 20 registros
                .ToListAsync();

            response.LecturasRecientes = lecturasRecientes.Select(ToDto).ToList();

            return Ok(response);
        }
    }
}