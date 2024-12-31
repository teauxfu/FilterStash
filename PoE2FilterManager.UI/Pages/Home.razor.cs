using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using PoE2FilterManager.Data;
using PoE2FilterManagerData;
using System.Diagnostics;

namespace PoE2FilterManagerUI.Pages
{
    public sealed partial class Home : ComponentBase, IAsyncDisposable, IDisposable
    {
        AppSettings? _config;
        IQueryable<Package>? _packages;
        private SyncService.PackageItem[]? _contents;
        private bool disposedValue;
        QuickGrid<Package>? _packagesGrid;
        QuickGrid<SyncService.PackageItem>? _contentsGrid;

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

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            // shouldn't have to do this... some kind of js bug in quickgrid or webview i guess
            try
            {
                if (_packagesGrid is not null)
                    await _packagesGrid.DisposeAsync();
                if (_contentsGrid is not null)
                    await _contentsGrid.DisposeAsync();
            }
            catch (JSException)
            {
                // pass
            }

            // Dispose of unmanaged resources.
            Dispose(false);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Home()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}