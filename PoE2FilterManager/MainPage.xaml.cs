using Microsoft.AspNetCore.Components.WebView.Maui;
using PoE2FilterManager.UI;
using PoE2FilterManagerData;
using System.Diagnostics;

namespace PoE2FilterManager
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnOpenPoE2Folder(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", AppSettings.DefaultFiltersPath);
        }

        private void OnOpenCacheFolder(object sender, EventArgs e)
        {

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

            // Add the new BlazorWebView to the container
            BlazorContainer.Children.Add(newBlazorWebView);
        }
    }
}
