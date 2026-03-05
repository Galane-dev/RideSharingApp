using RideSharing.Application.Interfaces;
using RideSharing.Domain.Entities;
using RideSharing.Infrastructure.Persistence;
using RideSharing.Infrastructure.Repositories;

namespace RideSharing.Infrastructure.UnitOfWork
{
    /// <summary>
    /// Coordinates all repository operations and flushes them to JSON files in a single Commit call.
    /// Loads all data once on construction; repositories work against the shared in-memory lists.
    /// </summary>
    public class JsonUnitOfWork : IUnitOfWork
    {
        private readonly JsonFileService _fileService;

        // Shared in-memory lists that all repositories operate against.
        private readonly List<Passenger> _passengers;
        private readonly List<Driver> _drivers;
        private readonly List<Ride> _rides;

        public IPassengerRepository Passengers { get; }
        public IDriverRepository Drivers { get; }
        public IRideRepository Rides { get; }

        public JsonUnitOfWork(JsonFileService fileService)
        {
            _fileService = fileService;

            _passengers = _fileService.Load<Passenger>(DataPaths.PassengerFile);
            _drivers = _fileService.Load<Driver>(DataPaths.DriverFile);
            _rides = _fileService.Load<Ride>(DataPaths.RideFile);

            Passengers = new PassengerRepository(_passengers);
            Drivers = new DriverRepository(_drivers);
            Rides = new RideRepository(_rides);
        }

        /// <summary>
        /// Persists all in-memory collections to their corresponding JSON files atomically.
        /// Call this once per business transaction, not after every individual repository operation.
        /// </summary>
        public void Commit()
        {
            _fileService.Save(DataPaths.PassengerFile, _passengers);
            _fileService.Save(DataPaths.DriverFile, _drivers);
            _fileService.Save(DataPaths.RideFile, _rides);
        }
    }
}
