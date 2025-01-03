using Microsoft.AspNetCore.Components.WebView.Maui;
using PoE2FilterManager.UI;
using PoE2FilterManager.Data;
using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Components.Web;
using PoE2FilterManager.Data.Services;

namespace PoE2FilterManager
{
    public partial class MainPage : ContentPage
    {
        private readonly MauiBridgeService _bridge;

        public MainPage(MauiBridgeService bridge)
        {
            _bridge = bridge;
            InitializeComponent();
        }

        private void OnOpenPoE2Folder(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Utils.DefaultFiltersPath);
        }

        private void OnOpenCacheFolder(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Utils.DefaultCachePath);
        }

        private void OnChangeBackground(object sender, EventArgs e)
        {
            _bridge.RaiseButtonClicked();
        }

        private void OnOpenAbout(object sender, EventArgs e)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version!;
            DisplayAlert($"FilterStash v{version.Major}.{version.Minor}.{version.Build}", "https://github.com/teauxfu/filterstash" , "Ok");
        }
        

        private void OnRestartBlazorAppClicked(object sender, EventArgs e)
        {
            // Remove the existing BlazorWebView
            BlazorContainer.Children.Clear();

            // Create a new BlazorWebView instance
            var newBlazorWebView = new BlazorWebView
            {
                HostPage = "wwwroot/index.html"
            };

            newBlazorWebView.RootComponents.Add(new RootComponent
            {
                Selector = "#app",
                ComponentType = typeof(Routes)
            });
            newBlazorWebView.RootComponents.Add(new RootComponent
            {
                Selector = "head::after",
                ComponentType = typeof(HeadOutlet)
            });
          

            // Add the new BlazorWebView to the container
            BlazorContainer.Children.Add(newBlazorWebView);
        }
    }
}
