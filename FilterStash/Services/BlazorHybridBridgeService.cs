namespace FilterStash.Services
{
    /// <summary>
    /// Provides communication across the MAUI-Blazor boundary. 
    /// </summary>
    /// <remarks>Currently used to support the OS native menu.</remarks>
    public class BlazorHybridBridgeService
    {
        public event EventHandler? ChangeBackgroundButtonClicked;

        public void RaiseButtonClicked()
        {
            ChangeBackgroundButtonClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
