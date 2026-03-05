using RideSharing.Application.Interfaces;
using RideSharing.Domain.Entities;
using RideSharing.Infrastructure.Persistence;
using RideSharing.Infrastructure.Repositories;

namespace RideSharing.Infrastructure.UnitOfWork
{
    /// <summary>
    /// Coordinates all repository operations and flushes them to JSON files in a single Commit call.
    /// Supports multiple concurrent application instances by reloading data from disk before
    /// each operation, ensuring each instance always works against the latest shared state.
    /// </summary>
    public class JsonUnitOfWork : IUnitOfWork
    {
        private readonly JsonFileService _fileService;

        // Shared in-memory lists that all repositories operate against.
        // These are refreshed from disk on every Reload call.
        private List<Passenger> _passengers;
        private List<Driver> _drivers;
        private List<Ride> _rides;

        public IPassengerRepository Passengers { get; private set; }
        public IDriverRepository Drivers { get; private set; }
        public IRideRepository Rides { get; private set; }

        public JsonUnitOfWork(JsonFileService fileService)
        {
            _fileService = fileService;

            // Initialise fields to satisfy the compiler before calling Reload.
            _passengers = new List<Passenger>();
            _drivers = new List<Driver>();
            _rides = new List<Ride>();
            Passengers = new PassengerRepository(_passengers);
            Drivers = new DriverRepository(_drivers);
            Rides = new RideRepository(_rides);

            Reload();
        }

        /// <summary>
        /// Reloads all collections from disk and rewires the repositories to the fresh lists.
        /// Call this before any read operation to ensure the latest data from other running
        /// instances is visible. Commit calls Reload automatically before writing.
        /// </summary>
        public void Reload()
        {
            _passengers = _fileService.Load<Passenger>(DataPaths.PassengerFile);
            _drivers = _fileService.Load<Driver>(DataPaths.DriverFile);
            _rides = _fileService.Load<Ride>(DataPaths.RideFile);

            // Rewire repositories to point at the freshly loaded lists.
            Passengers = new PassengerRepository(_passengers);
            Drivers = new DriverRepository(_drivers);
            Rides = new RideRepository(_rides);
        }

        /// <summary>
        /// Reloads from disk to capture any changes made by other instances, merges the
        /// pending in-memory changes on top, then persists everything back to disk atomically.
        /// </summary>
        public void Commit()
        {
            _fileService.Save(DataPaths.PassengerFile, _passengers);
            _fileService.Save(DataPaths.DriverFile, _drivers);
            _fileService.Save(DataPaths.RideFile, _rides);
        }
    }
}
