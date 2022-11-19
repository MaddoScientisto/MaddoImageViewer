using MaddoImageLib.Platforms.Windows;

using MaddoImager.ViewModels;

using MaddoServices.Services.Impl;
using MaddoServices.Services;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui;

namespace MaddoImager
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .UseMauiCommunityToolkit();

#if DEBUG
            builder.Logging.AddDebug();
#endif

#if WINDOWS
            builder.Services.AddTransient<IFolderPicker, FolderPicker>();
#elif MACCATALYST
            builder.Services.AddTransient<IFolderPicker, Platforms.MacCatalyst.FolderPicker>();
#endif

            //builder.Services.AddTransient<App>();

            builder.Services.AddTransient<IFisherYatesService, FisherYatesService>();
            builder.Services.AddTransient<IDbService, LiteDbService>();

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainViewModel>();


            return builder.Build();
        }
    }
}