using FilterStash.Services;
using Serilog;

namespace FilterStash
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
               .WriteTo.Debug()
               .CreateLogger();

            Log.Logger.Information("Hello, world! Serilog is configured");
            builder.UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

            if (!Directory.Exists(Utils.DefaultFiltersPath))
                Directory.CreateDirectory(Utils.DefaultFiltersPath);
            if (!Directory.Exists(Utils.DefaultCachePath))
                Directory.CreateDirectory(Utils.DefaultCachePath);

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddSingleton<BlazorHybridBridgeService>();
            builder.Services.AddSingleton<IIndexService>(new JsonIndexService(Utils.DefaultIndexPath));
            builder.Services.AddSingleton<ISyncService, GitHubSyncService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            //builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
