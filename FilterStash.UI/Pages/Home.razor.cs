using FilterStash.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.IO.Compression;
using System.Reflection;

namespace FilterStash.UI.Pages
{
    /// <summary>
    /// Holds the main UI for the app.
    /// Also holds a lot of app logic, such as moving files around, 
    /// which sould really be pulled out to a separate service.
    /// There's also a couple blocks that update the poe2 ini file crudely,
    /// which should also probably be done is a separate service.
    /// </summary>
    public sealed partial class Home : ComponentBase, IAsyncDisposable, IDisposable
    {
        private List<Package> _packages = [];
        IQueryable<PackageDisplay>? _displayPackages;
        // current selection
        Package? _currentPackage;
        string? _error;
        private readonly HttpClient http = new();

        // debugging, grid refs trying to mitigate js object disposal err on f5 reload
        QuickGrid<PackageItem>? _contentsGrid;

        [Inject] IIndexService IndexService { get; set; } = default!;
        [Inject] ISyncService SyncService { get; set; } = default!;
        [Inject] ILogger<Home> Log { get; set; } = default!;
        [Inject] BlazorHybridBridgeService Bridge { get; set; } = default!;

        [CascadingParameter] public Theme? ThemeContext { get; set; }

        bool showAddForm;

        void HandleThemeChange(object? sender, EventArgs e)
        {
            if (ThemeContext is Theme theme
                && !string.IsNullOrWhiteSpace(theme.BackgroundImage))
            {
                int? i = theme.BgUrls.IndexOf(theme.BackgroundImage);
                i = i == theme.BgUrls.Count - 1 ? 0 : i + 1;
                if (i is not null)
                    theme.BackgroundImage = theme.BgUrls[i.Value];
            }
        }

        async Task ReloadPackages()
        {
            try
            {
                var index = IndexService.ReadIndex();
                _packages = [.. index.Packages.Select(kvp => kvp.Value)];
                _displayPackages = _packages.Select(p => new PackageDisplay(p)).AsQueryable();

                _updatePending = await HandleCheckForUpdates();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "An error occurred while reading app config");
                _error = "failed to load app config";
                // complain 
            }
        }

        protected override async Task OnInitializedAsync()
        {
            Bridge.ChangeBackgroundButtonClicked += HandleThemeChange;
            await ReloadPackages();
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

        async Task InstallPackage(string name)
        {
            var index = IndexService.ReadIndex();
            if (!index.Packages.TryGetValue(name, out Package? package))
                throw new KeyNotFoundException($"The given package name [{name}] was not in the package index");

            if (!string.IsNullOrWhiteSpace(index.CurrentlyInstalledPackage))
            {
                await UninstallPackage(index.CurrentlyInstalledPackage);
                index = IndexService.ReadIndex();
            }

            string packageDir = Path.Combine(Utils.DefaultCachePath, name);
            if (!Directory.Exists(packageDir))
                throw new DirectoryNotFoundException($"The given package directory [{packageDir}] could not be found");

            foreach (var file in Directory.EnumerateFiles(packageDir))
            {
                string fileName = Path.GetFileName(file);
                string newPath = Path.Combine(Utils.DefaultFiltersPath, fileName);
                File.Copy(file, newPath, overwrite: true);
            }

            index.CurrentlyInstalledPackage = name;
            IndexService.SaveIndex(index);

            // HACK move this to a separete service dedicated to updating the ini file
            string poeini = Path.Combine(Utils.DefaultFiltersPath, "poe2_production_Config.ini");
            if (File.Exists(poeini))
            {
                try
                {
                    string[] lines = File.ReadAllLines(poeini);
                    List<string> linesOut = [];
                    foreach (string line in lines)
                        if (line.StartsWith("item_filter="))
                            linesOut.Add($"item_filter={package.Items.FirstOrDefault(i => i.Name.EndsWith(".filter", StringComparison.OrdinalIgnoreCase))?.Name}");
                        else
                            linesOut.Add(line);
                    File.WriteAllLines(poeini, linesOut);
                }
                catch (Exception ex)
                {
                    Log.LogError(ex, "An error occurred while installing a package");
                }
            }
        }

        private async Task UninstallPackage(string name)
        {
            string packageDir = Path.Combine(Utils.DefaultCachePath, name);
            if (!Directory.Exists(packageDir))
                throw new DirectoryNotFoundException($"The given package directory [{packageDir}] could not be found");

            // CONSIDER iterating over index.Packages[name].Items.Select(i => i.Name) instead
            foreach (var file in Directory.EnumerateFiles(packageDir))
            {
                string fileName = Path.GetFileName(file);
                string toDelete = Path.Combine(Utils.DefaultFiltersPath, fileName);
                if (File.Exists(toDelete))
                    File.Delete(toDelete);
            }

            var index = IndexService.ReadIndex();
            index.CurrentlyInstalledPackage = null;
            IndexService.SaveIndex(index);

            // HACK move this to a separete service dedicated to updating the ini file
            // NOTE there's a couple calls in kernel32.dll you can use to interact with this without rewriting the file, or pull in a library
            string poeini = Path.Combine(Utils.DefaultFiltersPath, "poe2_production_Config.ini");
            if (File.Exists(poeini))
            {
                try
                {
                    string[] lines = File.ReadAllLines(poeini);
                    List<string> linesOut = [];
                    foreach (string line in lines)
                        if (line.StartsWith("item_filter="))
                            linesOut.Add($"item_filter=");
                        else
                            linesOut.Add(line);
                    File.WriteAllLines(poeini, linesOut);
                }
                catch (Exception ex)
                {
                    Log.LogError(ex, "An error occurred while uninstalling a package");
                }
                {
                    // TODO show an alert or something?
                }
            }

            await ReloadPackages();
        }

        async Task HandleAddPackage(Package package)
        {
            bool success = false;
            if (package.SourceIsGitHub)
            {
                Log.LogInformation("GitHub package detected, attempting to download package files from source {source}", package.Source);
                await DownloadPackageFiles(package);
                success = true;
            }
            else if (package.SourceIsLocal)
            {
                // this is handled inside the add filter form now...
                // either all the handling should be up here (preferable, but more boilerplate)
                // or it should all be in there (probably less clean, but quicker)
                success = true;
            }
            
            if(success)
            {
                var index = IndexService.ReadIndex();
                index.Packages[package.Name] = package;
                IndexService.SaveIndex(index);
                showAddForm = false;
                await ReloadPackages();
            }
        }

        async Task HandleUpdatePackage(string name, bool force = false)
        {
            var index = IndexService.ReadIndex();
            var package = index.Packages[name];
            if (package.SourceIsGitHub)
            {
                if (await SyncService.GetFilterPackageAsync(package.Name, package.Source) is Package update)
                {
                    index.Packages[package.Name] = update;
                    await DownloadPackageFiles(package, force);
                    IndexService.SaveIndex(index);
                    index = IndexService.ReadIndex();
                }
                
                if(name == index.CurrentlyInstalledPackage)
                    await InstallPackage(name);
            }
            
            await ReloadPackages();
        }

        async Task DeletePackage(string name)
        {
            try
            {
                var index = IndexService.ReadIndex();
                if (index.CurrentlyInstalledPackage == name)
                {
                    await UninstallPackage(name);
                    index = IndexService.ReadIndex();
                }


                string packageCacheDir = Path.Combine(Utils.DefaultCachePath, name);
                if (Directory.Exists(packageCacheDir))
                    Directory.Delete(packageCacheDir, true);
                index.Packages.Remove(name);
                IndexService.SaveIndex(index);
                await ReloadPackages();
            }

            catch (Exception ex)
            {
                Log.LogError(ex, "An error occurred while deleting a package");
                throw;
            }
        }

        static readonly string[] allowedExtensions = [
            ".filter",
            ".md",
            ".txt",
            ".mp3",
            ".wav",
        ];
        async Task DownloadPackageFiles(Package package, bool force = false)
        {
            foreach (PackageItem item in package.Items
                .Where(i => allowedExtensions.Contains(Path.GetExtension(i.Name))))
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
        async Task<List<string>> HandleCheckForUpdates()
        {
            var index = IndexService.ReadIndex();
            List<string> updateAvailable = [];
            foreach (var package in index.Packages.Values.Where(p => p.SourceIsGitHub))
            {
                var p = await SyncService.GetFilterPackageAsync(package.Name, package.Source);
                if (string.IsNullOrWhiteSpace(package.LastCommitSha)
                    || package.LastCommitSha != p?.LastCommitSha)
                    updateAvailable.Add(package.Name);
            }
            return updateAvailable;
        }

        #region disposable
        // idisposable 
        bool disposedValue;
        private List<string> _updatePending = [];

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