using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace BusBuddy;

public partial class DashboardPage : ContentPage
{
    public ObservableCollection<RouteSummary> RecentRoutes { get; set; } = new()
    {
        new RouteSummary { Name = "Route 1", Date = "2024-06-01" },
        new RouteSummary { Name = "Route 2", Date = "2024-06-02" }
    };

    public DashboardPage()
    {
        InitializeComponent();
        BindingContext = this;
    }
}

public class RouteSummary
{
    public string Name { get; set; }
    public string Date { get; set; }
}
