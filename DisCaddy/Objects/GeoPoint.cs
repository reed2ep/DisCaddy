using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisCaddy.Objects
{
    public class GeoPoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public GeoPoint() { }

        public GeoPoint(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public Location ToLocation() => new Location(Latitude, Longitude);

        public static GeoPoint FromLocation(Location loc) => new(loc.Latitude, loc.Longitude);
    }
}
