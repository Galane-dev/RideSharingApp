using RideSharing.Domain.Entities;

namespace RideSharing.Application.Interfaces
{
    /// <summary>
    /// Driver-specific repository extending the generic contract.
    /// </summary>
    public interface IDriverRepository : IRepository<Driver>
    {
        /// <summary>Returns all drivers whose availability flag is true.</summary>
        List<Driver> GetAvailableDrivers();

        /// <summary>Returns the driver whose username matches, or null.</summary>
        Driver? GetByUsername(string username);

        /// <summary>Returns drivers whose average rating falls below the low-rating threshold.</summary>
        List<Driver> GetLowRatedDrivers();
    }
}
