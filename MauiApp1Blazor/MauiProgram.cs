using Microsoft.AspNetCore.Components.WebView.Maui;
using MauiApp1Blazor.Data;
using MauiApp1Blazor.Services;
using MauiApp1Blazor.Services.Impl;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;

namespace MauiApp1Blazor;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .RegisterBlazorMauiWebView()
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddBlazorWebView();
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
