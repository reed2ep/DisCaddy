namespace DisCaddy.Views;

public partial class MapPage : ContentPage
{
	public MapPage()
	{
		InitializeComponent();
		BindingContext(new MapViewModel());
	}
}