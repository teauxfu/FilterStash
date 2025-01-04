using FilterStash;
using FilterStash.Services;
using FilterStash.UI;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection;
using System.Text;
using Octokit;

namespace WinFormsShell
{
    public partial class Form1 : Form
    {

        BlazorWebView? blazor;
        BlazorHybridBridgeService _bridge = new();

        public Form1()
        {
            InitializeComponent();
            var version = Assembly.GetExecutingAssembly().GetName().Version!;
            Text = $"FilterStash v{version.Major}.{version.Minor}.{version.Build}";
            LoadBlazor();
        }

        private void LoadBlazor()
        {
            panel1.Controls.Remove(blazor);
            blazor?.Dispose();

            var services = new ServiceCollection();
            services.AddWindowsFormsBlazorWebView();

            // try to move this stuff to program.cs
            services.AddSingleton(_bridge);
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
            MessageBox.Show(caption: Utils.GetVersionString(), text: Utils.GetAboutText());
        }

        private void changeBackgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _bridge.RaiseButtonClicked();
        }

        private async void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var gh = new GitHubSyncService();
            if (await gh.GetLatestReleaseAsync() is not Release latest)
            {
                Process.Start("explorer.exe", "https:github.com/teauxfu/filterstash/releases/latest");
                return;
            }    
            else if (!Utils.GetVersionString().Contains(latest.TagName))
            {
                var dlg = MessageBox.Show($"You are running {Utils.GetVersionString()}. There is a new version {latest.TagName} available. Do you want to download it now?", 
                    "Update available",
                    MessageBoxButtons.YesNo
                );
                if(dlg == DialogResult.Yes && !string.IsNullOrWhiteSpace(latest.Assets[0].BrowserDownloadUrl))
                {
                    Process.Start("explorer.exe", latest.Assets[0].BrowserDownloadUrl);
                    return;
                }
            }
            else
            {
                MessageBox.Show($"You are running the latest version {Utils.GetVersionString()}.");
            }
        }
    }
}
