using RideSharing.Application.Interfaces;
using RideSharing.Domain.Entities;

namespace RideSharing.Infrastructure.Repositories
{
    /// <summary>
    /// In-memory passenger repository backed by a shared list managed by the Unit of Work.
    /// Does not perform any file I/O; the Unit of Work handles persistence on Commit.
    /// </summary>
    public class PassengerRepository : IPassengerRepository
    {
        private readonly List<Passenger> _passengers;

        public PassengerRepository(List<Passenger> passengers)
        {
            _passengers = passengers;
        }

        /// <summary>Returns all registered passengers.</summary>
        public List<Passenger> GetAll() => _passengers;

        /// <summary>Returns the passenger with the given ID, or null.</summary>
        public Passenger? GetById(Guid id)
            => _passengers.FirstOrDefault(p => p.Id == id);

        /// <summary>Returns the passenger with the given username, or null.</summary>
        public Passenger? GetByUsername(string username)
            => _passengers.FirstOrDefault(p =>
                string.Equals(p.Username, username, StringComparison.OrdinalIgnoreCase));

        /// <summary>Adds a new passenger to the collection.</summary>
        public void Add(Passenger passenger) => _passengers.Add(passenger);

        /// <summary>Replaces an existing passenger record matched by ID.</summary>
        public void Update(Passenger passenger)
        {
            var index = _passengers.FindIndex(p => p.Id == passenger.Id);

            if (index >= 0)
                _passengers[index] = passenger;
        }

        /// <summary>Removes the passenger with the given ID.</summary>
        public void Delete(Guid id) => _passengers.RemoveAll(p => p.Id == id);
    }
}
