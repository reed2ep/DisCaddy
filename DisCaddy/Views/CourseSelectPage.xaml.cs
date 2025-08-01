using DisCaddy.Repository.Interfaces;
using DisCaddy.Models;

namespace DisCaddy.Views;

public partial class CourseSelectPage : ContentPage
{
	public CourseSelectPage(ICourseRepository repo)
	{
		InitializeComponent();
        BindingContext = new CourseSelectionViewModel(repo);
    }
}