using Microsoft.Maui.Controls;

namespace BusBuddy;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        // Removed DI-based MainPage injection. ShellContent is now defined in XAML.
    }
}
