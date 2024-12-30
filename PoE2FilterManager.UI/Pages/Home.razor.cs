using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using PoE2FilterManager.Data;
using PoE2FilterManagerData;
using System.Diagnostics;

namespace PoE2FilterManagerUI.Pages
{
    public partial class Home : ComponentBase
    {
        AppSettings? _config;
        IQueryable<Package>? _packages;
        private SyncService.PackageItem[]? _contents;

        [Inject] IConfiguration Configuration { get; set; } = default!;
        [Inject] SyncService SyncService { get; set; } = default!;

        protected override void OnInitialized()
        {
            try
            {
                _config = Configuration.GetSection("AppSettings").Get<AppSettings>();
                _packages = _config?.Packages.AsQueryable();
            }
            catch (Exception ex)
            {
                ;
                // complain
            }
        }

        async Task HandleViewPackage(string source)
        {
            _contents = null;
            try
            {
                _contents = await SyncService.GetFilterPackageAsync(source);
            }
            catch
            {

            }
        }

        void HandleOpenPoeFolder()
        {
            if (Directory.Exists(_config?.FiltersPath))
                Process.Start("explorer.exe", _config.FiltersPath);
        }

        void HandleOpenConfigFile()
        {
            string path;
#if DEBUG
            path = "appsettings.json";
            Process.Start("explorer.exe", path);
            return;
#endif

            path = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    nameof(PoE2FilterManager)
            );
            Process.Start("explorer.exe", path);
        }
    }
}