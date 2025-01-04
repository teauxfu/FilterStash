using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic.Logging;
using PoE2FilterManager.Data;
using PoE2FilterManager.Data.Services;
using PoE2FilterManager.UI;
using System.Diagnostics;

namespace WinFormsShell
{
    public partial class Form1 : Form
    {

        BlazorWebView? blazor;

        public Form1()
        {
            InitializeComponent();
            LoadBlazor();
        }

        private void LoadBlazor()
        {
            panel1.Controls.Remove(blazor);

            var services = new ServiceCollection();
            services.AddWindowsFormsBlazorWebView();

            // try to move this stuff to program.cs
            services.AddSingleton<BlazorHybridBridgeService>();
            services.AddSingleton<ISyncService, GitHubSyncService>();
            services.AddSingleton<IIndexService>(new JsonIndexService(Utils.DefaultIndexPath));

            blazor = new() { HostPage = @"wwwroot\index.html", Dock = DockStyle.Fill };
            blazor.RootComponents.Add<Routes>("#app");
            blazor.Services = services.BuildServiceProvider();

            panel1.Controls.Add(blazor);
        }

        private void openPoE2FolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Directory.CreateDirectory(Utils.DefaultFiltersPath);
            Process.Start("explorer.exe", Utils.DefaultFiltersPath);
        }

        private void openCacheFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Directory.CreateDirectory(Utils.DefaultCachePath);
            Process.Start("explorer.exe", Utils.DefaultCachePath);
        }

        private void restartFilterStashToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadBlazor();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
