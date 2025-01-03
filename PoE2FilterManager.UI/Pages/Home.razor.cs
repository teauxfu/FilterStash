using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using PoE2FilterManager.Data;
using PoE2FilterManager.Data.Services;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace PoE2FilterManager.UI.Pages
{
    public sealed partial class Home : ComponentBase, IAsyncDisposable, IDisposable
    {
        private List<Package> _packages = [];
        IQueryable<PackageDisplay>? _displayPackages;
        // current selection
        Package? _currentPackage;
        string? _error;

        static string? GetVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version!;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }

        // debugging, grid refs trying to mitigate js object disposal err on f5 reload
        QuickGrid<PackageItem>? _contentsGrid;

        [Inject] IIndexService IndexService { get; set; } = default!;
        [Inject] ISyncService SyncService { get; set; } = default!;
        [Inject] ILogger<Home> Log { get; set; } = default!;
        [Inject] MauiBridgeService Bridge { get; set; } = default!;

        [CascadingParameter] public Theme? ThemeContext { get; set; }

        bool showAddForm;

        void HandleThemeChange(object? sender, EventArgs e)
        {
            if (ThemeContext is Theme theme
                && !string.IsNullOrWhiteSpace(theme.BackgroundImage))
            {
                int? i = theme.BgUrls.IndexOf(theme.BackgroundImage);
                i = (i == theme.BgUrls.Count - 1) ? 0 : i + 1;
                if (i is not null)
                    theme.BackgroundImage = theme.BgUrls[i.Value];
            }
        }

        protected override void OnInitialized()
        {
            Bridge.ChangeBackgroundButtonClicked += HandleThemeChange;

            try
            {
                var index = IndexService.ReadIndex();
                _packages = [.. index.Packages.Select(kvp => kvp.Value)];
                _displayPackages = _packages.Select(p => new PackageDisplay(p)).AsQueryable();
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "An error occurred while reading app config");
                _error = "failed to load app config";
                // complain 
            }
        }


        void HandleViewPackage(string name)
        {
            _currentPackage = null;
            var index = IndexService.ReadIndex();
            if (index.Packages.TryGetValue(name, out Package? package))
                _currentPackage = package;
        }

        void HandleClosePackageDetail()
            => _currentPackage = null;

        // todo move this index logic to the index service

        async Task HandleAddPackage(Package package)
        {
            var index = IndexService.ReadIndex();
            index.Packages[package.Name] = package;
            await DownloadPackageFiles(package);
            IndexService.SaveIndex(index);
            showAddForm = false;
            StateHasChanged();
        }

        async Task HandleUpdatePackage(string name, bool force = false)
        {
            var index = IndexService.ReadIndex();
            var package = index.Packages[name];
            if (await SyncService.GetFilterPackageAsync(package.Name, package.Source) is Package update)
            {
                index.Packages[package.Name] = update;
                await DownloadPackageFiles(package);
                IndexService.SaveIndex(index);
            }
            StateHasChanged();
        }

        async Task HandleRedownloadPackage(string name)
        {
            await HandleUpdatePackage(name, force: true);
        }

        private readonly HttpClient http = new();

        async Task DownloadPackageFiles(Package package, bool force = false)
        {
            foreach (PackageItem item in package.Items)
            {
                string packageDir = Path.Combine(Utils.DefaultCachePath, package.Name);
                Directory.CreateDirectory(packageDir);
                string dlPath = Path.Combine(packageDir, item.Name);

                if (!force && File.Exists(dlPath))
                {
                    FileInfo info = new(dlPath);
                    if (info.Length == item.Size)
                        return;
                }
                try
                {
                    if (!force && !string.IsNullOrEmpty(item.B64Content))
                    {
                        File.WriteAllBytes(dlPath, Convert.FromBase64String(item.B64Content));
                    }
                    else
                    {
                        using var stream = await http.GetStreamAsync(item.DownloadUrl);
                        using FileStream outputFile = new(dlPath, FileMode.Create, FileAccess.Write, FileShare.None);
                        await stream.CopyToAsync(outputFile);
                    }
                }
                catch (Exception ex)
                {
                    Log.LogError(ex, "An error occurred while downloading package files");  
                }
            }
        }


        #region disposable
        // idisposable 
        bool disposedValue;
        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            // shouldn't have to do this... some kind of js bug in quickgrid or webview i guess
            try
            {

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
                    Bridge.ChangeBackgroundButtonClicked -= HandleThemeChange;
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
        #endregion
    }
}