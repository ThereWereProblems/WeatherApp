using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp.Models
{
    internal class Locate
    {
        public double lat;
        public double lon;

        public Locate(double lat, double lon)
        {
            this.lat = lat;
            this.lon = lon;
        }
    }
}
