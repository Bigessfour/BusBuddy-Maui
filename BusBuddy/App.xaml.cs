using Syncfusion.Licensing;

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
        return new Window(new AppShell());
    }
}