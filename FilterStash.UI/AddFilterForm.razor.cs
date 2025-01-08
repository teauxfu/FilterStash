using FilterStash.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using System.IO.Compression;

namespace FilterStash.UI
{
    public partial class AddFilterForm : ComponentBase, IDisposable
    {
        [Parameter] public EventCallback<Package> OnValidSubmit { get; set; }

        EditContext EditContext = default!;
        ValidationMessageStore validationMessageStore = default!;
        PackageEditModel editModel = new();
        string? _warning;

        // used to check if name in use, consider taking a list of current package names as a param instead
        [Inject] IIndexService IndexService { get; set; } = default!;
        [Inject] ISyncService SyncService { get; set; } = default!;

        protected override void OnInitialized()
        {
            EditContext = new(editModel);
            validationMessageStore = new(EditContext);
            EditContext.OnFieldChanged += HandleFormChangedAsync;
        }

        async void HandleFormChangedAsync(object? sender, FieldChangedEventArgs e)
        {
            validationMessageStore.Clear();

            var index = IndexService.ReadIndex();
            if (index.Packages.ContainsKey(editModel.Name))
                validationMessageStore.Add(new FieldIdentifier(editModel, nameof(editModel.Name)), "The given filter name is already in use.");

            Package p = new(editModel.Name, editModel.Source);
            if (!EditContext.GetValidationMessages().Any())
            {
                if (p.SourceIsGitHub)
                {
                    if (await SyncService.GetFilterPackageAsync(editModel.Name, editModel.Source) is Package package)
                    {
                        bool filterFileMissing = !package.Items.Where(p => p.Name.EndsWith(".filter", StringComparison.OrdinalIgnoreCase)).Any();
                        if (filterFileMissing)
                            _warning = "It looks like this package source exists, but doesn't contain a filter file. Did you paste the right link?";
                        else
                            _warning = null;
                    }
                    else
                    {
                    }
                }
                else if (editModel.FileAttachment is not null)
                {
                    string[] allowedExtensions = [".zip", ".filter"];

                    if (!allowedExtensions.Contains(Path.GetExtension(editModel.FileAttachment.Name), StringComparer.OrdinalIgnoreCase))
                    {
                        validationMessageStore.Add(new FieldIdentifier(editModel, nameof(editModel.FileAttachment)), "You must select a .filter or .zip file to import from");
                    }


                    // check if zip contains a filter file
                    //using var archive = ZipFile.OpenRead(editModel.Source);
                    //bool filterFileMissing = !archive.Entries.Where(e => e.FullName.EndsWith(".filter", StringComparison.OrdinalIgnoreCase)).Any();
                    //if (filterFileMissing)
                    //    validationMessageStore.Add(new FieldIdentifier(editModel, nameof(editModel.Source)), "The given source was invalid. It looks like this zip file exists, but doesn't contain a .filter file. Did you paste the right path?");
                    //else
                    //    _warning = null;
                }
                else
                {
                    validationMessageStore.Add(new FieldIdentifier(editModel, nameof(editModel.Source)), "Please provide a valid GitHub repo, or a zip file.");
                }
            }
        }

        string? _dub;
        private MemoryStream? _fileStream;
        void HandleFileChange(InputFileChangeEventArgs e)
        {
            editModel.FileAttachment = e.File;
            EditContext.NotifyFieldChanged(new FieldIdentifier(editModel, nameof(editModel.FileAttachment)));
        }


        void HandleClearForm()
        {
            editModel.Name = editModel.Source = string.Empty;
            editModel.FileAttachment = null;
            EditContext.NotifyValidationStateChanged();
        }

        private class PackageEditModel
        {
            [Required] public string Name { get; set; } = string.Empty;
            public string Source { get; set; } = string.Empty;

            public IBrowserFile? FileAttachment { get; set; }
        }

        async Task HandleValidSubmitAsync()
        {
            Package p = new(editModel.Name, editModel.Source);

            if (p.SourceIsGitHub && await SyncService.GetFilterPackageAsync(editModel.Name, editModel.Source)
                is Package package)
            {
                if (OnValidSubmit.HasDelegate)
                    await OnValidSubmit.InvokeAsync(package);
            }

            if (!p.SourceIsGitHub && editModel.FileAttachment is not null)
            {
                // make sure there's a place in the cache for this incoming data
                string cachePath = Path.Combine(Utils.DefaultCachePath, p.Name);
                Directory.CreateDirectory(cachePath);

                // copy the file to temp so we can work with a path
                string temp = Path.GetTempFileName();
                await using (var fileStream = new FileStream(temp, FileMode.Create, FileAccess.Write))
                    await editModel.FileAttachment.OpenReadStream().CopyToAsync(fileStream);

                if (editModel.FileAttachment.Name.EndsWith(".filter", StringComparison.OrdinalIgnoreCase))
                {
                    string dst = Path.Combine(cachePath, editModel.FileAttachment.Name);
                    File.Copy(temp, dst, overwrite: true);
                    p.Source = dst;
                }
                else if (editModel.FileAttachment.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    ZipFile.ExtractToDirectory(temp, cachePath, overwriteFiles: true);
                    // Check if the extracted files contain a folder with the same name and pull the children up a level
                    string zipShellDir = Path.Combine(cachePath, Path.GetFileNameWithoutExtension(editModel.FileAttachment.Name));
                    if (Directory.Exists(zipShellDir))
                    {
                        foreach (var file in Directory.GetFiles(zipShellDir))
                            File.Move(file, Path.Combine(cachePath, Path.GetFileName(file)), overwrite: true);
                        Directory.Delete(zipShellDir);
                    }
                    p.Source = Directory.EnumerateFiles(cachePath).FirstOrDefault(f => f.EndsWith(".filter", StringComparison.OrdinalIgnoreCase)) ?? 
                        "unknown";
                }

                File.Delete(temp);

                // Add the extracted files to the package items
                p.Items = Directory.GetFiles(cachePath, "*.*", SearchOption.AllDirectories)
                    .Select(filePath => new PackageItem(
                        Name: Path.GetFileName(filePath),
                        HtmlUrl: Path.Combine(Utils.DefaultCachePath, Path.GetFileName(filePath)),
                        DownloadUrl: Path.Combine(Utils.DefaultCachePath, Path.GetFileName(filePath)),
                        Sha: Utils.GetFileSha1Hash(filePath),
                        Size: (int)new FileInfo(filePath).Length,
                        B64Content: string.Empty
                        )
                    ).ToList();
                p.LastUpdated = DateTimeOffset.Now;

                if (OnValidSubmit.HasDelegate)
                    await OnValidSubmit.InvokeAsync(p);
            }
        }

        void IDisposable.Dispose()
        {
            if (EditContext is not null)
                EditContext.OnFieldChanged -= HandleFormChangedAsync;
        }
    }
}