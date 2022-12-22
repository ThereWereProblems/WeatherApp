using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Entity;
using WeatherApp.Models;

namespace WeatherApp
{
    internal class DbInitializer
    {
        private readonly ApplicationDbContext _dbContext;

        public DbInitializer(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Seed()
        {

            if (_dbContext.Database.CanConnect())
            {
                if (!_dbContext.Cities.Any())
                {
                    var cities = GetCities();
                    foreach (var item in cities)
                    {
                        _dbContext.Cities.Add(item);
                        _dbContext.SaveChanges();
                    }
                }
            }
        }

        private IEnumerable<City> GetCities()
        {
            var roles = new List<City>()
            {
                new City("Białystok", 53.105010, 23.167102),
                new City("Bydgoszcz", 53.117735, 18.007736),
                new City("Gdańsk", 54.343229, 18.628444),
                new City("Gorzów Wielkopolski", 52.720899, 15.251780),
                new City("Katowice", 50.249859, 19.017038),
                new City("Kielce", 50.851569, 20.621034),
                new City("Kraków", 50.056293, 19.944799),
                new City("Lublin", 51.230485, 22.570839),
                new City("Łódź", 51.750786, 19.459598),
                new City("Olsztyn", 53.770330, 20.485700),
                new City("Opole", 50.656383, 17.944921),
                new City("Poznań", 52.383711, 16.920281),
                new City("Rzeszów", 50.000451, 21.999156),
                new City("Szczecin", 53.408263, 14.541918),
                new City("Toruń", 53.003502, 18.627307),
                new City("Warszawa", 52.199253, 21.022641),
                new City("Wrocław", 51.085213, 17.048414),
                new City("Zielona Góra", 51.922847, 15.498385)
            };
            return roles;
        }
    }
}
