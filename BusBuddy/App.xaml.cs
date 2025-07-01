using Syncfusion.Licensing;
using Microsoft.Maui.Controls;

namespace BusBuddy;

public partial class App : Application
{
    public IServiceProvider Services { get; }

    public App(IServiceProvider services)
    {
        Services = services;
        InitializeComponent();
        string? licenseKey = Environment.GetEnvironmentVariable("SYNCFUSION_LICENSE_KEY");
        if (!string.IsNullOrEmpty(licenseKey))
        {
            SyncfusionLicenseProvider.RegisterLicense(licenseKey);
        }
        else
        {
            // Optionally log or handle missing license key
        }
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new AppShell());
#if WINDOWS
        // If you need to maximize or set window state, use Syncfusion or WinUI APIs as needed
#endif
        return window;
    }
}