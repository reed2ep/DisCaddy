using System.Threading.Tasks;
using DisCaddy;
using Microsoft.Extensions.DependencyInjection;

namespace DisCaddy.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly IServiceProvider _serviceProvider;
        public MainPage(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
        }

        private async void OnBagPageClicked(object sender, EventArgs e)
        {
            var bagPage = _serviceProvider.GetService<BagPage>();
            await Navigation.PushAsync(bagPage);
        }

        private async void OnMapPageClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MapPage());
        }

        private async void OnCourseSelectPageClicked(object sender, EventArgs e)
        {
            var courseSelectPage = _serviceProvider.GetService<CourseSelectPage>();
            await Navigation.PushAsync(courseSelectPage);
        }
    }

}
