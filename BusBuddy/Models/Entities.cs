namespace BusBuddy.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        // Add other properties as needed
    }

    public class Driver
    {
        public int Id { get; set; }
        public string DriverName { get; set; } = string.Empty;
        // Add other properties as needed
    }

    public class Vehicle
    {
        public int Id { get; set; }
        public string BusNumber { get; set; } = string.Empty;
        // Add other properties as needed
    }

    public class Fuel
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public double Amount { get; set; }
        // Add other properties as needed
    }

    public class Maintenance
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public string Description { get; set; } = string.Empty;
        // Add other properties as needed
    }

    public class SchoolCalendar
    {
        public int Id { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        // Add other properties as needed
    }

    public class ActivitySchedule
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public DateTime ScheduledDate { get; set; }
        // Add other properties as needed
    }
}