using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;

using MaddoImageLib.Platforms.Windows;

using MaddoImageSorter.Data;

using MaddoServices.Services;
using MaddoServices.Services.Impl;

using Microsoft.Extensions.Logging;

namespace MaddoImageSorter
{
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
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif
#if WINDOWS
            builder.Services.AddTransient<IFolderPicker, FolderPicker>();
#elif MACCATALYST
		builder.Services.AddTransient<IFolderPicker, Platforms.MacCatalyst.FolderPicker>();
#endif

            //builder.Services.AddTransient<App>();

            builder.Services.AddSingleton<WeatherForecastService>();
            builder.Services.AddTransient<IFisherYatesService, FisherYatesService>();
            builder.Services.AddTransient<IDbService, LiteDbService>();

            builder.Services.AddBlazorise(options =>
                {
                    options.Immediate = true;
                }).AddBootstrap5Providers()
                .AddFontAwesomeIcons();
            return builder.Build();
        }
    }
}