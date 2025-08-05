using DisCaddy.Repository.Interfaces;
using DisCaddy.Models;

namespace DisCaddy.Views;

public partial class CourseSelectPage : ContentPage
{
    private readonly ICourseRepository _courseRepo;
    private readonly IHoleRepository _holeRepo;
    public CourseSelectPage(ICourseRepository repo)
	{
		InitializeComponent();
        BindingContext = new CourseSelectionViewModel(repo);
    }
    private async void OnCourseCreateClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MapPage(_courseRepo, _holeRepo));
    }
}