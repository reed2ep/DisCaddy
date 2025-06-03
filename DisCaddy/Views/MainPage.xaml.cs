using System.Threading.Tasks;
using DisCaddy;

namespace DisCaddy.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnBagPageClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BagPage());
        }
    }

}
