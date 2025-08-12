using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DisCaddy.Objects
{
    public class Hole
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string TeeLocationJson { get; set; }
        public string BasketLocationJson { get; set; }
        public string FlightPathJson { get; set; }

        public int CourseId { get; set; }
        public int HoleNumber { get; set; }
        public int Par { get; set; }
        public double LengthFeet { get; set; }

        [Ignore]
        public GeoPoint TeeLocation
        {
            get => JsonSerializer.Deserialize<GeoPoint>(TeeLocationJson);
            set => TeeLocationJson = JsonSerializer.Serialize(value);
        }

        [Ignore]
        public GeoPoint BasketLocation
        {
            get => JsonSerializer.Deserialize<GeoPoint>(BasketLocationJson);
            set => BasketLocationJson = JsonSerializer.Serialize(value);
        }

        [Ignore]
        public List<GeoPoint> FlightPath
        {
            get => string.IsNullOrEmpty(FlightPathJson)
                ? new List<GeoPoint>()
                : JsonSerializer.Deserialize<List<GeoPoint>>(FlightPathJson);
            set => FlightPathJson = JsonSerializer.Serialize(value);
        }

    }
}
