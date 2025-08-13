using System;
using System.Collections.Generic;
using System.Linq;
using DisCaddy.Models;
using Microsoft.Maui.Maps;
using CommunityToolkit.Maui.Maps;
using Microsoft.Maui.Controls.Maps;
using DisCaddy.Repository.Interfaces;
using DisCaddy.Objects;
using Microsoft.Maui.Devices.Sensors;
#if ANDROID
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
#endif

namespace DisCaddy.Views;

public partial class MapPage : ContentPage
{
    List<Location> flightPathPoints = new();

    public MapPage(ICourseRepository courseRepo, IHoleRepository holeRepo)
    {
        InitializeComponent();
        var vm = new MapViewModel(courseRepo, holeRepo);
        BindingContext = vm;

        vm.CenterMapRequested += loc =>
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var path = vm.CurrentHole?.FlightPath?
                    .Select(p => new Location(p.Latitude, p.Longitude))
                    .ToList();

                if (path != null && path.Count >= 2)
                {
                    CenterPath(path);

                    double minLat = path.Min(p => p.Latitude);
                    double maxLat = path.Max(p => p.Latitude);
                    double minLon = path.Min(p => p.Longitude);
                    double maxLon = path.Max(p => p.Longitude);
                    var center = new Location((minLat + maxLat) / 2, (minLon + maxLon) / 2);

                    var bearing = (float)BearingDegrees(path[0], path[1]);
                    RotateMapTo(center, bearing);
                }
                else
                {
                    var pts = new List<Location> { loc };
                    if (vm.CurrentHole?.BasketLocation is GeoPoint b)
                        pts.Add(new Location(b.Latitude, b.Longitude));

                    CenterPath(pts);

                    if (pts.Count >= 2)
                    {
                        double minLat = pts.Min(p => p.Latitude);
                        double maxLat = pts.Max(p => p.Latitude);
                        double minLon = pts.Min(p => p.Longitude);
                        double maxLon = pts.Max(p => p.Longitude);
                        var center = new Location((minLat + maxLat) / 2, (minLon + maxLon) / 2);

                        var bearing = (float)BearingDegrees(pts[0], pts[1]);
                        RotateMapTo(center, bearing);
                    }
                }
            });

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

    static double BearingDegrees(Location from, Location to)
    {
        double r = Math.PI / 180.0;
        double phi1 = from.Latitude * r;
        double phi2 = to.Latitude * r;
        double dLambda = (to.Longitude - from.Longitude) * r;
        double y = Math.Sin(dLambda) * Math.Cos(phi2);
        double x = Math.Cos(phi1) * Math.Sin(phi2) - Math.Sin(phi1) * Math.Cos(phi2) * Math.Cos(dLambda);
        return (Math.Atan2(y, x) * 180.0 / Math.PI + 360.0) % 360.0;
    }

    void RotateMapTo(Location target, float bearing)
    {
#if ANDROID
        var handler = MyMap.Handler;
        if (handler?.PlatformView is Android.Gms.Maps.MapView mv)
        {
            mv.GetMapAsync(new MapReadyCallback(g =>
            {
                var current = g.CameraPosition;
                var cam = new CameraPosition(new LatLng(target.Latitude, target.Longitude), current.Zoom, current.Tilt, bearing);
                g.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cam));
            }));
        }
#else
        MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(target, Distance.FromMeters(50)));
#endif
    }

#if ANDROID
sealed class MapReadyCallback : Java.Lang.Object, IOnMapReadyCallback
{
    readonly Action<GoogleMap> _ready;
    public MapReadyCallback(Action<GoogleMap> ready) => _ready = ready;
    public void OnMapReady(GoogleMap googleMap) => _ready?.Invoke(googleMap);
}
#endif

    private void DrawFlightPath()
    {
        MyMap.MapElements.Clear();
        var polyline = new Microsoft.Maui.Controls.Maps.Polyline { StrokeColor = Colors.Blue, StrokeWidth = 4 };
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
            total += Location.CalculateDistance(flightPathPoints[i - 1], flightPathPoints[i], DistanceUnits.Miles);
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

        var parText = await DisplayPromptAsync("Set Par", "Enter par for this hole:", initialValue: "3", keyboard: Keyboard.Numeric);
        if (parText is null) return;
        if (!int.TryParse(parText, out var par) || par < 2 || par > 8)
        {
            await DisplayAlert("Invalid", "Enter a whole number between 2 and 8.", "OK");
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
            Par = par,
            LengthFeet = CalculateTotalDistance()
        };

        await vm.AddHoleAsync(hole);
        await DisplayAlert("Saved", $"Hole {hole.HoleNumber} saved.", "OK");

        flightPathPoints.Clear();
        MyMap.MapElements.Clear();
        DistanceLabel.Text = "Total Distance: 0 ft";
    }

    public void CenterPath(IReadOnlyList<Location> points)
    {
        if (points == null || points.Count == 0) return;

        double minLat = points.Min(p => p.Latitude);
        double maxLat = points.Max(p => p.Latitude);
        double minLon = points.Min(p => p.Longitude);
        double maxLon = points.Max(p => p.Longitude);

        var center = new Location((minLat + maxLat) / 2, (minLon + maxLon) / 2);

        double latDelta = (maxLat - minLat);
        double lonDelta = (maxLon - minLon);

        const double pad = 1.2;
        latDelta *= pad;
        lonDelta *= pad;

        const double minDelta = 0.0005;
        if (latDelta < minDelta) latDelta = minDelta;
        if (lonDelta < minDelta) lonDelta = minDelta;

        var span = new MapSpan(center, latDelta, lonDelta);
        MyMap.MoveToRegion(span);
    }
}
