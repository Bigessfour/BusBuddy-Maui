namespace BusBuddy.Models
{
    public class Route
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int AMDriverId { get; set; }
        public int AMVehicleId { get; set; }
        // Add other properties as needed
    }
}