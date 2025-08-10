using DisCaddy.Models;
using Microsoft.Maui.Maps;
using CommunityToolkit.Maui.Maps;
using Microsoft.Maui.Controls.Maps;
using DisCaddy.Repository.Interfaces;
using DisCaddy.Objects;

namespace DisCaddy.Views;

public partial class MapPage : ContentPage
{
    List<Location> flightPathPoints = new();

    public MapPage(ICourseRepository courseRepo, IHoleRepository holeRepo)
    {
        InitializeComponent();
        var vm = new MapViewModel(courseRepo, holeRepo);
        BindingContext = vm;

        vm.CenterMapRequested += location =>
            MainThread.BeginInvokeOnMainThread(() =>
                MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(location, Distance.FromMeters(50))));

        vm.FlightPathRequested += geoPoints =>
            MainThread.BeginInvokeOnMainThread(() =>
            {
                flightPathPoints.Clear();
                foreach (var gp in geoPoints)
                    flightPathPoints.Add(new Location(gp.Latitude, gp.Longitude));
                DrawFlightPath();
            });

        MyMap.MapClicked += (s, e) =>
        {
            flightPathPoints.Add(e.Location);
            DrawFlightPath();
        };
    }

    private void DrawFlightPath()
    {
        MyMap.MapElements.Clear();

        var polyline = new Polyline { StrokeColor = Colors.Blue, StrokeWidth = 4 };
        foreach (var p in flightPathPoints) polyline.Geopath.Add(p);
        MyMap.MapElements.Add(polyline);

        DistanceLabel.Text = $"Total Distance: {CalculateTotalDistance():0} ft";
    }

    private void OnClearClicked(object sender, EventArgs e)
    {
        flightPathPoints.Clear();
        MyMap.MapElements.Clear();
        DistanceLabel.Text = "Total Distance: 0 ft";
    }

    private double CalculateTotalDistance()
    {
        double total = 0;
        for (int i = 1; i < flightPathPoints.Count; i++)
        {
            total += Location.CalculateDistance(flightPathPoints[i - 1],
                                                flightPathPoints[i],
                                                DistanceUnits.Miles);
        }
        return total * 5280;
    }

    private async void OnSaveHole(object sender, EventArgs e)
    {
        if (BindingContext is not MapViewModel vm) return;

        if (vm.SelectedCourse == null)
        {
            await DisplayAlert("Error", "Create or select a course first.", "OK");
            return;
        }

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
            Par = 3,
            LengthFeet = CalculateTotalDistance()
        };

        await vm.AddHoleAsync(hole);
        await DisplayAlert("Saved", $"Hole {hole.HoleNumber} saved.", "OK");

        flightPathPoints.Clear();
        MyMap.MapElements.Clear();
        DistanceLabel.Text = "Total Distance: 0 ft";
    }
}