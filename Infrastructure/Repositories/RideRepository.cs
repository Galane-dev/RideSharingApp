using RideSharing.Application.Interfaces;
using RideSharing.Domain.Entities;
using RideSharing.Domain.Enums;

namespace RideSharing.Infrastructure.Repositories
{
    /// <summary>
    /// In-memory ride repository backed by a shared list managed by the Unit of Work.
    /// Does not perform any file I/O; the Unit of Work handles persistence on Commit.
    /// </summary>
    public class RideRepository : IRideRepository
    {
        private readonly List<Ride> _rides;

        public RideRepository(List<Ride> rides)
        {
            _rides = rides;
        }

        /// <summary>Returns all rides in the system.</summary>
        public List<Ride> GetAll() => _rides;

        /// <summary>Returns the ride with the given ID, or null.</summary>
        public Ride? GetById(Guid id)
            => _rides.FirstOrDefault(r => r.Id == id);

        /// <summary>Returns all rides currently awaiting a driver.</summary>
        public List<Ride> GetOpenRequests()
            => _rides.Where(r => r.Status == RideStatus.Requested).ToList();

        /// <summary>Returns all rides that belong to the specified passenger, ordered most recent first.</summary>
        public List<Ride> GetRidesForPassenger(Guid passengerId)
            => _rides
                .Where(r => r.PassengerId == passengerId)
                .OrderByDescending(r => r.RequestedAt)
                .ToList();

        /// <summary>Returns all rides assigned to the specified driver, ordered most recent first.</summary>
        public List<Ride> GetRidesForDriver(Guid driverId)
            => _rides
                .Where(r => r.DriverId == driverId)
                .OrderByDescending(r => r.RequestedAt)
                .ToList();

        /// <summary>
        /// Returns the ride currently in Accepted status for a driver, or null.
        /// A driver should have at most one active ride at any time.
        /// </summary>
        public Ride? GetActiveRideForDriver(Guid driverId)
            => _rides.FirstOrDefault(r => r.DriverId == driverId && r.Status == RideStatus.Accepted);

        /// <summary>Adds a new ride to the collection.</summary>
        public void Add(Ride ride) => _rides.Add(ride);

        /// <summary>Replaces an existing ride record matched by ID.</summary>
        public void Update(Ride ride)
        {
            var index = _rides.FindIndex(r => r.Id == ride.Id);

            if (index >= 0)
                _rides[index] = ride;
        }

        /// <summary>Removes the ride with the given ID.</summary>
        public void Delete(Guid id) => _rides.RemoveAll(r => r.Id == id);
    }
}
