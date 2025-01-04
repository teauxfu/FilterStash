using Serilog;
using WinFormsShell;
using Log = Serilog.Log;

namespace FilterStash.WinForms
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();



            // TODO configure logging (read settings in from config)
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.Console()
               .WriteTo.Debug()
               .CreateLogger();

            Log.Logger.Information("Hello, world! Serilog is configured");


            if (!Directory.Exists(Utils.DefaultFiltersPath))
                Directory.CreateDirectory(Utils.DefaultFiltersPath);
            if (!Directory.Exists(Utils.DefaultCachePath))
                Directory.CreateDirectory(Utils.DefaultCachePath);

            Application.Run(new Form1());
        }
    }
}