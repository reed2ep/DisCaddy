namespace DisCaddy.Views;
using System.Net.Http;

public partial class HoleSetupPage : ContentPage
{
    private CancellationTokenSource _cancelTokenSource;
    public double? currentHeading = null;
    public HoleSetupPage()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetCurrentLocation();
        await StartCompass();

    }

    public async Task GetWeatherData(double lat, double lon)
	{
		var config = ConfigLoader.Load();
		string key = config.OpenWeatherApiKey;

        string uri = $"https://api.openweathermap.org/data/3.0/onecall?lat={lat}&lon={lon}&exclude=minutely,hourly,daily,alerts&appid={key}";


        HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync(uri);

        if (response.IsSuccessStatusCode)
        {
            string json = await response.Content.ReadAsStringAsync();
            Console.WriteLine(json); 
        }
    }

    public async Task GetCurrentLocation()
    {
        try
        {
            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            _cancelTokenSource = new CancellationTokenSource();
            Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

            if (location != null)
                await GetWeatherData(location.Latitude, location.Longitude);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Location Access Failed");
        }
    }

    public async Task StartCompass()
    {
        if(!Compass.Default.IsSupported)
        {
            Console.WriteLine("Compass Unsupported");
            return;
        }
        Compass.Default.ReadingChanged += Compass_ReadingChanged;
        Compass.Default.Start(SensorSpeed.UI);
        StopCompass();
    }

    private void Compass_ReadingChanged(object sender, CompassChangedEventArgs e)
    {
        CompassData data = e.Reading;
        currentHeading = data.HeadingMagneticNorth;
        Console.WriteLine($"Heading: {currentHeading}°");
    }

    public void StopCompass()
    {
        if (Compass.Default.IsMonitoring)
            Compass.Default.Stop();
    }
}