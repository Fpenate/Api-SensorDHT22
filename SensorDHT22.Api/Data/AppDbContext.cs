using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SensorDHT22.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace SensorDHT22.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) {}
         // ---> LÍNEA PARA EL SENSOR <---
        public DbSet<SensorReading> SensorReadings { get; set; }

    }
}