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

namespace DisCaddy;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		//register services + pages
		builder.Services.AddSingleton<IDiscRepository>(s =>
		{
			var dbPath = Path.Combine(FileSystem.AppDataDirectory, "discs.db3");
			return new DiscRepository(dbPath);
        });
		builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<BagPage>();
        builder.UseMauiApp<App>().UseMauiMaps().UseMauiCommunityToolkit().UseMauiCommunityToolkitMaps("AIzaSyBfKwcN4vU8wp8-Mxr0-juOqhQi0Amnuzs");

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
	}
}
