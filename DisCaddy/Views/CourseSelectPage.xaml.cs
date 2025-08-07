using DisCaddy.Repository.Interfaces;
using DisCaddy.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DisCaddy.Views;

public partial class CourseSelectPage : ContentPage
{
    private readonly IServiceProvider _serviceProvider;

    public CourseSelectPage(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;

        var courseRepo = _serviceProvider.GetRequiredService<ICourseRepository>();
        BindingContext = new CourseSelectionViewModel(courseRepo);
    }

    private async void OnCourseCreateClicked(object sender, EventArgs e)
    {
        var mapPage = _serviceProvider.GetRequiredService<MapPage>();
        await Navigation.PushAsync(mapPage);
    }
}