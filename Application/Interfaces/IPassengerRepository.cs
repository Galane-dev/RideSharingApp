using RideSharing.Domain.Entities;

namespace RideSharing.Application.Interfaces
{
    /// <summary>
    /// Passenger-specific repository extending the generic contract.
    /// </summary>
    public interface IPassengerRepository : IRepository<Passenger>
    {
        /// <summary>Returns the passenger whose username matches, or null.</summary>
        Passenger? GetByUsername(string username);
    }
}
