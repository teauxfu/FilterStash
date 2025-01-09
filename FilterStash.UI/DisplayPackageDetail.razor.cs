using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using System.Diagnostics;

namespace FilterStash.UI
{
    // there are two types of blocks in a filter file, the kind we ignore or the kind we care about
    // the kind we ignore can be snipped out and interpolated back in later, the kind we care about will be reserialize and then interpolated back in

    interface ISegment
    {
        string Content { get; }
    }
    
    class FluffSegment : ISegment
    {
        public string Content { get; set; }  = string.Empty;
    }

    class ShowSegment : ISegment
    {
        public List<string> Rules { get; set; } = [];

        public string Content  => string.Join(Environment.NewLine, Rules);

        public string? Rarity 
            => Rules.FirstOrDefault(r => r.StartsWith("Rarity", StringComparison.OrdinalIgnoreCase));
        public string? BaseType
            => Rules.FirstOrDefault(r => r.StartsWith("BaseType", StringComparison.OrdinalIgnoreCase));

        public string? CustomSoundAlert
            => Rules.FirstOrDefault(r => r.StartsWith("CustomAlertSound", StringComparison.OrdinalIgnoreCase));
    }

    class FilterFileStructure
    {
        public List<ISegment> Segments { get; set; } = [];
    }


    public partial class DisplayPackageDetail : ComponentBase
    {
        QuickGrid<PackageItem>? _contentsGrid;
        [Parameter, EditorRequired] public Package? Package { get; set; }
        [Parameter] public EventCallback<string> OnUpdatePackage { get; set; }
        [Parameter] public EventCallback<string> OnRedownloadPackage { get; set; }
        [Parameter] public List<string> UpdatePending { get; set; } = [];

        async Task HandleOnUpdatePackage(string name)
        {
            if (OnUpdatePackage.HasDelegate)
                await OnUpdatePackage.InvokeAsync(name);
        }

        async Task HandleOnRedownloadPackageAsync(string name)
        {
            if (OnRedownloadPackage.HasDelegate)
                await OnRedownloadPackage.InvokeAsync(name);
        }

        static bool isPriorityFile(string fileName)
                          => fileName.Equals("readme.md", StringComparison.OrdinalIgnoreCase)
                              | Path.GetExtension(fileName).Equals(".filter", StringComparison.OrdinalIgnoreCase);
        private static IQueryable<PackageItem> GetOrderedPackageItems(Package package) => (package.Items ?? [])
                .OrderBy(i => isPriorityFile(i.Name) ? 0 : 1)
                .ThenBy(i => i.Name)
                .AsQueryable();

        private void HandleOpenPackageFolder()
        {
            if (Package is not null)
            {
                string packageDir = Path.Combine(Utils.DefaultCachePath, Package.Name);
                Directory.CreateDirectory(packageDir);
                Process.Start("explorer.exe", packageDir);
            }
        }


        FilterFileStructure? filter = null;

        private void HandleSoundbites()
        {
            filter = null;
            if (Package is null)
                return;
            var filterFile = Package.Items.FirstOrDefault(i => i.Name.EndsWith(".filter", StringComparison.OrdinalIgnoreCase));
            if (filterFile is null)
                return;
            var filterPath = Path.Combine(Utils.DefaultCachePath, Package.Name, filterFile.Name);
            string content = File.ReadAllText(filterPath);

            filter = new();
            var showSegments = content.Split("Show", StringSplitOptions.TrimEntries);
            foreach (var segment in showSegments)
            {
                if (segment.StartsWith('#'))
                {
                    filter.Segments.Add(new FluffSegment { Content = segment });
                    continue;
                }
                else
                {
                    ShowSegment show = new() { Rules = [ "Show"] };
                    string[] lines = segment.Split(Environment.NewLine);
                    show.Rules.AddRange(lines);
                    filter.Segments.Add(show);
                }
            }
            StateHasChanged();
        }
    }
}