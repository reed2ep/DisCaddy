namespace DisCaddy.Views;
using System.Net.Http;

public partial class HoleSetupPage : ContentPage
{
	public HoleSetupPage()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetWeatherData();
    }

    public async Task GetWeatherData()
	{
		double lat = 40.6892;
		double lon = -74.0445;

		string part = "minutely,hourly,daily,alerts";
		var config = ConfigLoader.Load();
		string key = config.OpenWeatherApiKey;

        string uri = $"https://api.openweathermap.org/data/2.5/onecall?lat={lat}&lon={lon}&exclude={part}&appid={key}";


        HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync(uri);

        if (response.IsSuccessStatusCode)
        {
            string json = await response.Content.ReadAsStringAsync();
            Console.WriteLine(json); 
        }
    }
}