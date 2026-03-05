using RideSharing.Domain.Entities;
using RideSharing.Domain.Enums;

namespace RideSharing.Application.Interfaces
{
    /// <summary>
    /// Ride-specific repository extending the generic contract.
    /// </summary>
    public interface IRideRepository : IRepository<Ride>
    {
        /// <summary>Returns all rides that currently have the Requested status.</summary>
        List<Ride> GetOpenRequests();

        /// <summary>Returns all rides belonging to the specified passenger.</summary>
        List<Ride> GetRidesForPassenger(Guid passengerId);

        /// <summary>Returns all rides assigned to the specified driver.</summary>
        List<Ride> GetRidesForDriver(Guid driverId);

        /// <summary>Returns the active (Accepted) ride for a driver, or null.</summary>
        Ride? GetActiveRideForDriver(Guid driverId);
    }
}
