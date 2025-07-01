using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Extensions.Logging;

namespace BusBuddy;

public partial class MainPage : ContentPage
{
    private readonly ILogger<MainPage> _logger;
    private bool _formShown = false;

    public ObservableCollection<string> Vehicles { get; set; } = new();
    public ObservableCollection<string> Drivers { get; set; } = new();

    public MainPage(ILogger<MainPage> logger)
    {
        InitializeComponent();
        _logger = logger;
        AMVehiclePicker.ItemsSource = Vehicles;
        PMVehiclePicker.ItemsSource = Vehicles;
        AMDriverPicker.ItemsSource = Drivers;
        PMDriverPicker.ItemsSource = Drivers;
        SaveRouteButton.Clicked += OnSaveRouteClicked;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!_formShown)
        {
            await LoadVehiclesAndDrivers();
            CreateRouteForm.IsVisible = true;
            _formShown = true;
        }
    }

    private async Task LoadVehiclesAndDrivers()
    {
        // TODO: Replace with real DB calls
        Vehicles.Clear();
        Drivers.Clear();
        // Example data
        Vehicles.Add("Bus 1");
        Vehicles.Add("Bus 2");
        Drivers.Add("Driver 1");
        Drivers.Add("Driver 2");
        // If you have async DB calls, await them here and add results to Vehicles/Drivers
    }

    private void OnSaveRouteClicked(object sender, EventArgs e)
    {
        // Collect form data
        var date = RouteDatePicker.Date;
        var routeName = RouteNameEntry.Text;
        var amVehicle = AMVehiclePicker.SelectedItem as string;
        var amDriver = AMDriverPicker.SelectedItem as string;
        var pmVehicle = PMVehiclePicker.SelectedItem as string;
        var pmDriver = PMDriverPicker.SelectedItem as string;
        var amRiders = AMRidersEntry.Text;
        var pmRiders = PMRidersEntry.Text;
        // TODO: Save to DB
        CreateRouteForm.IsVisible = false;
        DisplayAlert("Saved", $"Route '{routeName}' created.", "OK");
    }
}
