using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Entity;

namespace WeatherApp
{
    internal class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base()
        {
        }

        public DbSet<City> Cities { get; set; }
        public DbSet<Query> Queries{ get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=DatabaseSWP;Trusted_Connection=True;");
        }
    }
}
