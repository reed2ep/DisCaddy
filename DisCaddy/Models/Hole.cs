using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisCaddy.Models
{
    public class Hole
    {
        public int Id { get; set; }
        public GeoPoint TeeLocation { get; set; }
        public GeoPoint BasketLocation { get; set; }
        public List<GeoPoint> FlightPath { get; set; } = new();
        public int Par { get; set; }
        public double LengthFeet { get; set; }
    }
}
