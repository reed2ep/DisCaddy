using DisCaddy.Models;

namespace DisCaddy.Views;
public partial class BagPage : ContentPage
{
	public BagPage(IDiscRepository repo)
	{
		InitializeComponent();
        BindingContext = new BagViewModel(repo);
    }
}