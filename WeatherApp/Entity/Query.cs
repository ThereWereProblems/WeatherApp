using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp.Entity
{
    internal class Query
    {
        public int Id { get; set; }
        public int CityId { get; set; }
        public virtual City City { get; set; }
        public DateTime Date { get; set; }
    }
}
