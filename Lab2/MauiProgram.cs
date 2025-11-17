using Microsoft.Extensions.Logging;
using System.Xml.Xsl;
using Lab2.Services;
using Lab2.ViewModels;
using Lab2.Views;

namespace Lab2;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<IXmlSearchStrategy, LinqToXmlSearchStrategy>();
        builder.Services.AddTransient<SaxSearchStrategy>();
        builder.Services.AddTransient<DomSearchStrategy>();
        builder.Services.AddTransient<LinqToXmlSearchStrategy>();
        builder.Services.AddSingleton<XmlSearchContext>();
        builder.Services.AddSingleton<XslTransformer>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}
