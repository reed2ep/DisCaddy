using DisCaddy.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Controls.Maps;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Maps;
using DisCaddy.Repository;
using DisCaddy.Repository.Interfaces;
using System.Text.Json;

namespace DisCaddy;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
        var config = ConfigLoader.Load();
        var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        builder.UseMauiMaps();
        builder.UseMauiCommunityToolkit();
        builder.UseMauiCommunityToolkitMaps(config.GoogleMapsApiKey);

        builder.Services.AddSingleton<IDiscRepository>(s =>
		{
			var dbPath = Path.Combine(FileSystem.AppDataDirectory, "discs.db3");
			return new DiscRepository(dbPath);
        });
		builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<BagPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
	}
}
