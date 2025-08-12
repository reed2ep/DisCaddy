using DisCaddy.Repository.Interfaces;
using DisCaddy.Models;
using Microsoft.Extensions.DependencyInjection;
using DisCaddy.Objects;

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

    private async void OnCourseSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Course selectedCourse)
        {
            var mapPage = _serviceProvider.GetRequiredService<MapPage>();

            if (mapPage.BindingContext is MapViewModel vm)
                await vm.LoadCourseAsync(selectedCourse);

            await Navigation.PushAsync(mapPage);

            ((CollectionView)sender).SelectedItem = null;
        }
    }
}