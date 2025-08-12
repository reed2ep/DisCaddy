using Microsoft.Extensions.DependencyInjection;
using DisCaddy.Views;
namespace DisCaddy
{
    public partial class App : Application
    {
        public App(IServiceProvider sp, IAppConfigProvider cfg)
        {
            InitializeComponent();
            MainPage = new NavigationPage(sp.GetRequiredService<MainPage>());
        }

        private static async Task WarmConfigAsync(IAppConfigProvider cfg)
        {
            var c = await cfg.GetAsync();
            System.Diagnostics.Debug.WriteLine($"[CFG] key prefix: {c.GoogleMapsApiKey?.Substring(0, 8)}");
        }
    }
}