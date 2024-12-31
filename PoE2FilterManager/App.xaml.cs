using System.Reflection;

namespace PoE2FilterManager
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            string? version = ((AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(
                Assembly.GetExecutingAssembly(), typeof(AssemblyFileVersionAttribute), false)!)
                ?.Version.ToString();

            return new Window(new NavigationPage(new MainPage()));
        }
    }
}
