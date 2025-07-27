using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisCaddy.Models
{
    public class MapViewModel
    {
        public MapViewModel()
        {
            GeoLocateCommand = new Command(async () => await GeoLocate());
        }

        private async Task GeoLocate()
        {
            var location = await Geolocation.GetLocationAsync();
            if (location != null)
            {
                MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                    new Location(location.Latitude, location.Longitude),
                    Distance.FromMeters(50)));
            }
        }
    }
}
