using RideSharing.Domain.Enums;

namespace RideSharing.Domain.Entities
{
    /// <summary>
    /// Represents a single ride transaction between a passenger and a driver.
    /// Stores PassengerId and DriverId rather than full navigation objects
    /// to prevent circular reference issues during JSON serialisation.
    /// </summary>
    public class Ride
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid PassengerId { get; set; }

        /// <summary>Null until a driver accepts the ride.</summary>
        public Guid? DriverId { get; set; }

        public string PickupLocation { get; set; } = string.Empty;

        public string DropoffLocation { get; set; } = string.Empty;

        public double DistanceKm { get; set; }

        public decimal Fare { get; set; }

        public RideStatus Status { get; set; } = RideStatus.Requested;

        /// <summary>Timestamp when the ride was first requested.</summary>
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Timestamp when the ride was completed. Null until completed.</summary>
        public DateTime? CompletedAt { get; set; }
    }
}
