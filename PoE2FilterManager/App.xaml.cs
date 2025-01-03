using PoE2FilterManager.Data.Services;
using System.Reflection;

namespace PoE2FilterManager
{
    public partial class App : Application
    {
        private readonly MauiBridgeService _bridge;

        public App(MauiBridgeService bridge)
        {
            _bridge = bridge;
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version!;
            var page = new NavigationPage(new MainPage(_bridge));
            return new Window(page) { Width = 1190, Height = 850, Title = $"FilterStash v{version.Major}.{version.Minor}.{version.Build}" } ;
        }
    }
}
