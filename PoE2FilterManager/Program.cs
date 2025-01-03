using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Serilog;
using PoE2FilterManager.Data;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.AspNetCore.Components.Web;
using PoE2FilterManager.Data.Services;

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

//            bool isDebug = false;
//#if DEBUG
//            // HACK don't have easy access to env.isdevelopment here on a mauibuilder 
//            isDebug = true;
//#endif

//            string configPath;
//            if (isDebug)
//                configPath = "appsettings.json";
//            else
//            {
//                configPath = Utils.DefaultIndexPath;
//                if(!Utils.ConfigIsValid())
//                    Utils.CreateDefaultConfig();
//            }

            // TODO run this sooner, then use it for initializing serilog
            //var config = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile(configPath, optional: false, reloadOnChange: true)
            //    .Build();
            //builder.Configuration.AddConfiguration(config);

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddSingleton<MauiBridgeService>();
            builder.Services.AddSingleton<IIndexService>(new JsonIndexService(Utils.DefaultIndexPath));
            builder.Services.AddSingleton<ISyncService, GitHubSyncService>();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
