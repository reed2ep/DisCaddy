using DisCaddy.Models;
using Microsoft.Maui.Maps;
using CommunityToolkit.Maui.Maps;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Controls.Shapes;
using DisCaddy.Repository.Interfaces;
using DisCaddy.Objects;
namespace DisCaddy.Views;

public partial class MapPage : ContentPage
{
    List<Location> flightPathPoints = new();
    public MapPage(ICourseRepository courseRepo, IHoleRepository holeRepo)
	{
		InitializeComponent();
		BindingContext = new MapViewModel(courseRepo, holeRepo);
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

    private async void OnCreateCourseClicked(object sender, EventArgs e)
    {
        if (BindingContext is MapViewModel vm)
        {
            // Hardcoded course name for now; replace with entry input later
            await vm.CreateCourseAsync();
            await DisplayAlert("Success", "Course created. You can now add holes.", "OK");
        }
    }

    private async void OnSaveHole(object sender, EventArgs e)
    {
        if (BindingContext is MapViewModel vm)
        {
            if (flightPathPoints.Count < 2)
            {
                await DisplayAlert("Error", "Add at least a tee and basket point.", "OK");
                return;
            }

            var tee = flightPathPoints.First();
            var basket = flightPathPoints.Last();
            var geoPath = flightPathPoints.Select(p => new GeoPoint(p.Latitude, p.Longitude)).ToList();

            var hole = new Hole
            {
                HoleNumber = vm.Holes.Count + 1,
                TeeLocation = new GeoPoint(tee.Latitude, tee.Longitude),
                BasketLocation = new GeoPoint(basket.Latitude, basket.Longitude),
                FlightPath = geoPath,
                Par = 3, // hardcoded for now; could add Entry
                LengthFeet = CalculateTotalDistance()
            };

            await vm.AddHoleAsync(hole);
            await DisplayAlert("Saved", $"Hole {hole.HoleNumber} saved.", "OK");

            // Reset the map
            flightPathPoints.Clear();
            MyMap.MapElements.Clear();
            DistanceLabel.Text = "Total Distance: 0 ft";
        }
    }
}