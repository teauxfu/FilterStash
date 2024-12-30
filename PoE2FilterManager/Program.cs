using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Serilog;
using PoE2FilterManager.Data;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using PoE2FilterManagerData;

namespace PoE2FilterManager
{
    public static class Program
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            
            // TODO configure logging (read settings in from config)
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.Console()
               .CreateLogger();

            Log.Logger.Information("Hello, world! Serilog is configured");
            builder.UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

            bool isDebug = false;
#if DEBUG
            // HACK don't have easy access to env.isdevelopment here on a mauibuilder 
            isDebug = true;
#endif

            string configPath;
            if (isDebug)
                configPath = "appsettings.json";
            else
            {
                configPath = Utils.DefaultSettingsPath;
                if(!Utils.ConfigIsValid())
                    Utils.CreateDefaultConfig();
            }

            // TODO run this sooner, then use it for initializing serilog
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(configPath, optional: false, reloadOnChange: true)
                .Build();

            builder.Configuration.AddConfiguration(config);
            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddSingleton<SyncService>();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
