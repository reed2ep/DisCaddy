using DisCaddy.Models;
using Microsoft.Maui.Maps;
namespace DisCaddy.Views;

public partial class MapPage : ContentPage
{
	public MapPage()
	{
		InitializeComponent();
		BindingContext = new MapViewModel();
        InitMapAsync();
    }

    private async Task InitMapAsync()
    {
        try
        {
            //var location = await Geolocation.GetLastKnownLocationAsync();

            // Example: Mt. Airy Disc Golf Course in Cincinnati
            var center = new Location(39.1724163, -84.5768989);
            if (center != null)
            {
                MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                    new Location(center),
                    Distance.FromMeters(100)));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unable to get location: {ex.Message}", "OK");
        }
    }
}