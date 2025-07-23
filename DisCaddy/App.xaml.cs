using Microsoft.Extensions.DependencyInjection;
using DisCaddy.Views;
namespace DisCaddy
{
    public partial class App : Application
    {
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            MainPage = new NavigationPage(serviceProvider.GetService<MainPage>());
        }
    }
}