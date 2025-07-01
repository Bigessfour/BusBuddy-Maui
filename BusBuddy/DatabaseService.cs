using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using BusBuddy.Models;

namespace BusBuddy
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        // New constructor to accept a full connection string
        public DatabaseService(string connectionString, bool isFullString)
        {
            _connectionString = connectionString;
        }

        public DatabaseService(string databaseName)
        {
            // Use Windows Authentication and trust server certificate
            _connectionString = $"Server=ST-LPTP9-23\\SQLEXPRESS01;Database={databaseName};Trusted_Connection=True;Encrypt=Optional;TrustServerCertificate=True;";
        }

        public async Task<DataTable> ExecuteQueryAsync(string query)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            var dataTable = new DataTable();
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            dataTable.Load(reader);
            return dataTable;
        }

        public async Task<int> ExecuteNonQueryAsync(string query)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync();
        }

        public async Task<List<string>> GetTableNamesAsync()
        {
            var tableNames = new List<string>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'", connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                tableNames.Add(reader.GetString(0));
            }
            return tableNames;
        }

        public async Task<DataTable> GetTableInfoAsync(string tableName)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand($"SELECT * FROM [{tableName}]", connection);
            var dataTable = new DataTable();
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            dataTable.Load(reader);
            return dataTable;
        }

        // Test connection method
        public static async Task<bool> TestConnectionAsync(string connectionString)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
                return connection.State == ConnectionState.Open;
            }
            catch
            {
                return false;
            }
        }

        // Create a new database if it does not exist
        public static async Task<bool> CreateDatabaseAsync(string server, string databaseName)
        {
            var connectionString = $"Server={server};Database=master;Trusted_Connection=True;Encrypt=Optional;TrustServerCertificate=True;";
            var checkDbQuery = $"IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{databaseName}') CREATE DATABASE [{databaseName}]";
            try
            {
                using var connection = new SqlConnection(connectionString);
                using var command = new SqlCommand(checkDbQuery, connection);
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Ensure the database and required tables are created if they do not exist
        public static async Task<bool> EnsureDatabaseAndTablesAsync(string server, string databaseName, string tableSchemaSql)
        {
            // Create database if it doesn't exist
            bool dbCreated = await CreateDatabaseAsync(server, databaseName);
            if (!dbCreated)
                return false;

            // Now connect to the new database and ensure tables exist
            var connectionString = $"Server={server};Database={databaseName};Trusted_Connection=True;Encrypt=Optional;TrustServerCertificate=True;";
            try
            {
                using var connection = new SqlConnection(connectionString);
                using var command = new SqlCommand(tableSchemaSql, connection);
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> CreateRouteAsync(Route route)
        {
            var sql = "INSERT INTO Routes (Name, AMDriverId, AMVehicleId) OUTPUT INSERTED.Id VALUES (@Name, @AMDriverId, @AMVehicleId)";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Name", route.Name);
            command.Parameters.AddWithValue("@AMDriverId", route.AMDriverId);
            command.Parameters.AddWithValue("@AMVehicleId", route.AMVehicleId);
            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result is int id ? id : throw new InvalidOperationException("Failed to insert route and retrieve the ID.");
        }

        public async Task<Route?> GetRouteAsync(int id)
        {
            var sql = "SELECT Id, Name, AMDriverId, AMVehicleId FROM Routes WHERE Id = @Id";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Route
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    AMDriverId = reader.GetInt32(2),
                    AMVehicleId = reader.GetInt32(3)
                };
            }
            return null;
        }

        public async Task<List<Route>> GetAllRoutesAsync()
        {
            var sql = "SELECT Id, Name, AMDriverId, AMVehicleId FROM Routes";
            var routes = new List<Route>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                routes.Add(new Route
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    AMDriverId = reader.GetInt32(2),
                    AMVehicleId = reader.GetInt32(3)
                });
            }
            return routes;
        }

        public async Task<bool> UpdateRouteAsync(Route route)
        {
            var sql = "UPDATE Routes SET Name = @Name, AMDriverId = @AMDriverId, AMVehicleId = @AMVehicleId WHERE Id = @Id";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Name", route.Name);
            command.Parameters.AddWithValue("@AMDriverId", route.AMDriverId);
            command.Parameters.AddWithValue("@AMVehicleId", route.AMVehicleId);
            command.Parameters.AddWithValue("@Id", route.Id);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteRouteAsync(int id)
        {
            var sql = "DELETE FROM Routes WHERE Id = @Id";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        // VEHICLE CRUD
        public async Task<int> CreateVehicleAsync(Vehicle vehicle)
        {
            var sql = "INSERT INTO Vehicles (BusNumber) OUTPUT INSERTED.Id VALUES (@BusNumber)";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@BusNumber", vehicle.BusNumber);
            await connection.OpenAsync();
            return (int)await command.ExecuteScalarAsync();
        }

        public async Task<Vehicle?> GetVehicleAsync(int id)
        {
            var sql = "SELECT Id, BusNumber FROM Vehicles WHERE Id = @Id";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Vehicle
                {
                    Id = reader.GetInt32(0),
                    BusNumber = reader.GetString(1)
                };
            }
            return null;
        }

        public async Task<List<Vehicle>> GetAllVehiclesAsync()
        {
            var sql = "SELECT Id, BusNumber FROM Vehicles";
            var vehicles = new List<Vehicle>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                vehicles.Add(new Vehicle
                {
                    Id = reader.GetInt32(0),
                    BusNumber = reader.GetString(1)
                });
            }
            return vehicles;
        }

        public async Task<bool> UpdateVehicleAsync(Vehicle vehicle)
        {
            var sql = "UPDATE Vehicles SET BusNumber = @BusNumber WHERE Id = @Id";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@BusNumber", vehicle.BusNumber);
            command.Parameters.AddWithValue("@Id", vehicle.Id);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteVehicleAsync(int id)
        {
            var sql = "DELETE FROM Vehicles WHERE Id = @Id";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        // DRIVER CRUD
        public async Task<int> CreateDriverAsync(Driver driver)
        {
            var sql = "INSERT INTO Drivers (DriverName) OUTPUT INSERTED.Id VALUES (@DriverName)";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@DriverName", driver.DriverName);
            await connection.OpenAsync();
            return (int)await command.ExecuteScalarAsync();
        }

        public async Task<Driver?> GetDriverAsync(int id)
        {
            var sql = "SELECT Id, DriverName FROM Drivers WHERE Id = @Id";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Driver
                {
                    Id = reader.GetInt32(0),
                    DriverName = reader.GetString(1)
                };
            }
            return null;
        }

        public async Task<List<Driver>> GetAllDriversAsync()
        {
            var sql = "SELECT Id, DriverName FROM Drivers";
            var drivers = new List<Driver>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                drivers.Add(new Driver
                {
                    Id = reader.GetInt32(0),
                    DriverName = reader.GetString(1)
                });
            }
            return drivers;
        }

        public async Task<bool> UpdateDriverAsync(Driver driver)
        {
            var sql = "UPDATE Drivers SET DriverName = @DriverName WHERE Id = @Id";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@DriverName", driver.DriverName);
            command.Parameters.AddWithValue("@Id", driver.Id);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteDriverAsync(int id)
        {
            var sql = "DELETE FROM Drivers WHERE Id = @Id";
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }
    }
}
