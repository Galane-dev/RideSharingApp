namespace RideSharing.Infrastructure.Persistence
{
    /// <summary>
    /// Centralised file paths for JSON data storage.
    /// Keeps path strings out of individual repositories.
    /// </summary>
    public static class DataPaths
    {
        public const string PassengerFile = "data/passengers.json";
        public const string DriverFile = "data/drivers.json";
        public const string RideFile = "data/rides.json";
    }
}
