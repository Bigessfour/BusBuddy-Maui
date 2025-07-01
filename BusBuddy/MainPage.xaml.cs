using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;

namespace BusBuddy;

public partial class MainPage : ContentPage
{
	int count = 0;
	private readonly ILogger<MainPage> _logger;
	public ObservableCollection<string> DataItems { get; set; } = new();

	public MainPage(ILogger<MainPage> logger)
	{
		InitializeComponent();
		_logger = logger;
		DataListView.ItemsSource = DataItems;
		_ = EnsureDatabaseSetup();
		_ = LoadData();
		// Run SQL schema test on load
		_ = TestSqlSchemaAsync();
	}

	private void OnCounterClicked(object? sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}

	private async Task TestSqlSchemaAsync()
	{
		// Example LocalDB connection string, adjust as needed
		string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=BusBuddy;Trusted_Connection=True;";
		bool canConnect = await DatabaseService.TestConnectionAsync(connectionString);
		if (!canConnect)
		{
			await DisplayAlert("SQL Test", "Failed to connect to database.", "OK");
			return;
		}
		var db = new DatabaseService(connectionString, true);
		try
		{
			var tables = await db.GetTableNamesAsync();
			string msg = tables.Count > 0 ? $"Tables: {string.Join(", ", tables)}" : "No tables found.";
			await DisplayAlert("SQL Schema Test", msg, "OK");
		}
		catch (Exception ex)
		{
			await DisplayAlert("SQL Test Error", ex.Message, "OK");
		}
	}

	private async void OnCreateDbClicked(object sender, EventArgs e)
	{
		string server = @"ST-LPTP9-23\\SQLEXPRESS01";
		string dbName = "BusBuddyTest"; // Change as needed
		try
		{
			bool created = await DatabaseService.CreateDatabaseAsync(server, dbName);
			if (created)
			{
				_logger?.LogInformation($"Database '{dbName}' created or already exists on {server}.");
				await DisplayAlert("Database", $"Database '{dbName}' created or already exists.", "OK");
			}
			else
			{
				_logger?.LogWarning($"Failed to create database '{dbName}' on {server}. No exception details.");
				await DisplayAlert("Database", $"Failed to create database '{dbName}'. (No exception details)", "OK");
			}
		}
		catch (Exception ex)
		{
			_logger?.LogError(ex, $"Exception while creating database '{dbName}' on {server}.");
			await DisplayAlert("Database Error", ex.Message, "OK");
		}
	}

	private async Task LoadData()
	{
		string connectionString = "Server=localhost\\SQLEXPRESS;Database=BusBuddy;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;";
		try
		{
			using var conn = new SqlConnection(connectionString);
			await conn.OpenAsync();
			var cmd = new SqlCommand("SELECT TOP 10 * FROM YourTable", conn); // Change 'YourTable' to your actual table name
			using var reader = await cmd.ExecuteReaderAsync();
			DataItems.Clear();
			while (await reader.ReadAsync())
			{
				var value = reader[0]?.ToString();
				if (!string.IsNullOrEmpty(value))
				{
					DataItems.Add(value);
				}
				else
				{
					_logger?.LogWarning("Null or empty value encountered in SQL result set.");
				}
			}
		}
		catch (Exception ex)
		{
			_logger?.LogError(ex, "Error loading data from SQL Server");
			await DisplayAlert("Data Load Error", ex.Message, "OK");
		}
	}

	private async Task EnsureDatabaseSetup()
	{
		string server = @"ST-LPTP9-23\\SQLEXPRESS01";
		string dbName = "BusBuddy";
		// Example table schema, adjust as needed
		string tableSchemaSql = @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='YourTable' and xtype='U')
		CREATE TABLE YourTable (
			Id INT IDENTITY(1,1) PRIMARY KEY,
			Name NVARCHAR(100) NULL
		)";
		bool ensured = await DatabaseService.EnsureDatabaseAndTablesAsync(server, dbName, tableSchemaSql);
		if (!ensured)
		{
			await DisplayAlert("Database Setup", "Failed to ensure database and tables exist.", "OK");
		}
	}

	// ROUTES CRUD
	private async void OnCreateRouteClicked(object sender, EventArgs e)
	{
		// TODO: Show UI to enter details or use dummy data
		var db = new DatabaseService("Server=localhost\\SQLEXPRESS;Database=BusBuddy;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;", true);
		var id = await db.CreateRouteAsync(new Models.Route { Name = "Route 1", AMDriverId = 1, AMVehicleId = 1 });
		await DisplayAlert("Create Route", $"Created Route with Id {id}", "OK");
	}
	private async void OnReadRouteClicked(object sender, EventArgs e)
	{
		var db = new DatabaseService("Server=localhost\\SQLEXPRESS;Database=BusBuddy;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;", true);
		var routes = await db.GetAllRoutesAsync();
		DataItems.Clear();
		foreach (var r in routes) DataItems.Add($"{r.Id}: {r.Name}");
	}
	private async void OnUpdateRouteClicked(object sender, EventArgs e)
	{
		var db = new DatabaseService("Server=localhost\\SQLEXPRESS;Database=BusBuddy;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;", true);
		var route = await db.GetRouteAsync(1); // Example: update route with Id 1
		if (route != null)
		{
			route.Name = "Updated Route";
			await db.UpdateRouteAsync(route);
			await DisplayAlert("Update Route", "Route updated.", "OK");
		}
		else
		{
			await DisplayAlert("Update Route", "Route not found.", "OK");
		}
	}
	private async void OnDeleteRouteClicked(object sender, EventArgs e)
	{
		var db = new DatabaseService("Server=localhost\\SQLEXPRESS;Database=BusBuddy;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;", true);
		var deleted = await db.DeleteRouteAsync(1); // Example: delete route with Id 1
		await DisplayAlert("Delete Route", deleted ? "Route deleted." : "Route not found.", "OK");
	}

	// BUSES CRUD
	private async void OnCreateBusClicked(object sender, EventArgs e)
	{
		// TODO: Show UI to enter details or use dummy data
		var db = new DatabaseService("Server=localhost\\SQLEXPRESS;Database=BusBuddy;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;", true);
		// Implement CreateVehicleAsync in DatabaseService
		var id = await db.CreateVehicleAsync(new Models.Vehicle { BusNumber = "Bus 1" });
		await DisplayAlert("Create Bus", $"Created Bus with Id {id}", "OK");
	}
	private async void OnReadBusClicked(object sender, EventArgs e)
	{
		var db = new DatabaseService("Server=localhost\\SQLEXPRESS;Database=BusBuddy;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;", true);
		var buses = await db.GetAllVehiclesAsync();
		DataItems.Clear();
		foreach (var b in buses) DataItems.Add($"{b.Id}: {b.BusNumber}");
	}
	private async void OnUpdateBusClicked(object sender, EventArgs e)
	{
		var db = new DatabaseService("Server=localhost\\SQLEXPRESS;Database=BusBuddy;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;", true);
		var bus = await db.GetVehicleAsync(1); // Example: update bus with Id 1
		if (bus != null)
		{
			bus.BusNumber = "Updated Bus";
			await db.UpdateVehicleAsync(bus);
			await DisplayAlert("Update Bus", "Bus updated.", "OK");
		}
		else
		{
			await DisplayAlert("Update Bus", "Bus not found.", "OK");
		}
	}
	private async void OnDeleteBusClicked(object sender, EventArgs e)
	{
		var db = new DatabaseService("Server=localhost\\SQLEXPRESS;Database=BusBuddy;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;", true);
		var deleted = await db.DeleteVehicleAsync(1); // Example: delete bus with Id 1
		await DisplayAlert("Delete Bus", deleted ? "Bus deleted." : "Bus not found.", "OK");
	}

	// DRIVERS CRUD
	private async void OnCreateDriverClicked(object sender, EventArgs e)
	{
		// TODO: Show UI to enter details or use dummy data
		var db = new DatabaseService("Server=localhost\\SQLEXPRESS;Database=BusBuddy;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;", true);
		var id = await db.CreateDriverAsync(new Models.Driver { DriverName = "Driver 1" });
		await DisplayAlert("Create Driver", $"Created Driver with Id {id}", "OK");
	}
	private async void OnReadDriverClicked(object sender, EventArgs e)
	{
		var db = new DatabaseService("Server=localhost\\SQLEXPRESS;Database=BusBuddy;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;", true);
		var drivers = await db.GetAllDriversAsync();
		DataItems.Clear();
		foreach (var d in drivers) DataItems.Add($"{d.Id}: {d.DriverName}");
	}
	private async void OnUpdateDriverClicked(object sender, EventArgs e)
	{
		var db = new DatabaseService("Server=localhost\\SQLEXPRESS;Database=BusBuddy;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;", true);
		var driver = await db.GetDriverAsync(1); // Example: update driver with Id 1
		if (driver != null)
		{
			driver.DriverName = "Updated Driver";
			await db.UpdateDriverAsync(driver);
			await DisplayAlert("Update Driver", "Driver updated.", "OK");
		}
		else
		{
			await DisplayAlert("Update Driver", "Driver not found.", "OK");
		}
	}
	private async void OnDeleteDriverClicked(object sender, EventArgs e)
	{
		var db = new DatabaseService("Server=localhost\\SQLEXPRESS;Database=BusBuddy;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;", true);
		var deleted = await db.DeleteDriverAsync(1); // Example: delete driver with Id 1
		await DisplayAlert("Delete Driver", deleted ? "Driver deleted." : "Driver not found.", "OK");
	}

	private async void OnTestButtonClicked(object sender, EventArgs e)
	{
		// Run all test logic here
		await TestSqlSchemaAsync();
		// Add more test calls as needed
		// await SomeOtherTestAsync();
	}
}
