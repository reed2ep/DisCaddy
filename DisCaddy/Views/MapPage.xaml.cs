using DisCaddy.Models;
using Microsoft.Maui.Maps;
using CommunityToolkit.Maui.Maps;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Controls.Shapes;
namespace DisCaddy.Views;

public partial class MapPage : ContentPage
{
    List<Location> flightPathPoints = new();
    public MapPage()
	{
		InitializeComponent();
		BindingContext = new MapViewModel();
        InitMapAsync();
        MyMap.MapClicked += (s, e) =>
        {
            var tappedPoint = e.Location;
            flightPathPoints.Add(tappedPoint);
            DrawFlightPath();
        };
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
                    Distance.FromMeters(50)));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unable to get location: {ex.Message}", "OK");
        }
    }

    private void DrawFlightPath()
    {
        MyMap.MapElements.Clear(); // clear old path

        var polyline = new Microsoft.Maui.Controls.Maps.Polyline
        {
            StrokeColor = Colors.Blue,
            StrokeWidth = 4
        };

        foreach (var point in flightPathPoints)
            polyline.Geopath.Add(point);

        MyMap.MapElements.Add(polyline);

        double totalFeet = CalculateTotalDistance();
        DistanceLabel.Text = $"Total Distance: {totalFeet:0} ft";
    }

    private void OnClearClicked(object sender, EventArgs e)
    {
        flightPathPoints.Clear();
        MyMap.MapElements.Clear();
    }

    private double CalculateTotalDistance()
    {
        double total = 0;
        for (int i = 1; i < flightPathPoints.Count; i++)
        {
            total += Location.CalculateDistance(
                flightPathPoints[i - 1],
                flightPathPoints[i],
                DistanceUnits.Miles);
        }
        return total * 5280;
    }
}