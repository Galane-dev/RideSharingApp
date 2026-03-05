using RideSharing.Application.Interfaces;
using RideSharing.Domain.Constants;
using RideSharing.Domain.Entities;

namespace RideSharing.Infrastructure.Repositories
{
    /// <summary>
    /// In-memory driver repository backed by a shared list managed by the Unit of Work.
    /// Does not perform any file I/O; the Unit of Work handles persistence on Commit.
    /// </summary>
    public class DriverRepository : IDriverRepository
    {
        private readonly List<Driver> _drivers;

        public DriverRepository(List<Driver> drivers)
        {
            _drivers = drivers;
        }

        /// <summary>Returns all registered drivers.</summary>
        public List<Driver> GetAll() => _drivers;

        /// <summary>Returns the driver with the given ID, or null.</summary>
        public Driver? GetById(Guid id)
            => _drivers.FirstOrDefault(d => d.Id == id);

        /// <summary>Returns the driver with the given username, or null.</summary>
        public Driver? GetByUsername(string username)
            => _drivers.FirstOrDefault(d =>
                string.Equals(d.Username, username, StringComparison.OrdinalIgnoreCase));

        /// <summary>Returns all drivers whose availability flag is currently true.</summary>
        public List<Driver> GetAvailableDrivers()
            => _drivers.Where(d => d.IsAvailable).ToList();

        /// <summary>
        /// Returns drivers whose average rating falls below the low-rating threshold.
        /// Only includes drivers who have received at least one rating.
        /// </summary>
        public List<Driver> GetLowRatedDrivers()
            => _drivers
                .Where(d => d.Rating > 0 && d.Rating < AppConstants.LowRatingThreshold)
                .ToList();

        /// <summary>Adds a new driver to the collection.</summary>
        public void Add(Driver driver) => _drivers.Add(driver);

        /// <summary>Replaces an existing driver record matched by ID.</summary>
        public void Update(Driver driver)
        {
            var index = _drivers.FindIndex(d => d.Id == driver.Id);

            if (index >= 0)
                _drivers[index] = driver;
        }

        /// <summary>Removes the driver with the given ID.</summary>
        public void Delete(Guid id) => _drivers.RemoveAll(d => d.Id == id);
    }
}
