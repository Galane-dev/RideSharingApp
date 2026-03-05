using RideSharing.Application.Interfaces;
using RideSharing.Domain.Constants;
using RideSharing.Domain.Entities;
using RideSharing.Domain.Enums;

namespace RideSharing.Application.Services
{
    /// <summary>
    /// Provides platform-wide analytics and reporting for administrators.
    /// </summary>
    public class ReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>Returns the total number of successfully completed rides.</summary>
        public int GetTotalCompletedRides()
            => _unitOfWork.Rides.GetAll().Count(r => r.Status == RideStatus.Completed);

        /// <summary>Returns the sum of all driver earnings across the platform.</summary>
        public decimal GetTotalPlatformEarnings()
            => _unitOfWork.Drivers.GetAll().Sum(d => d.Earnings);

        /// <summary>
        /// Returns the average rating across all rated drivers.
        /// Returns zero when no drivers have been rated yet.
        /// </summary>
        public double GetAverageDriverRating()
        {
            var ratedDrivers = _unitOfWork.Drivers.GetAll().Where(d => d.Rating > 0).ToList();

            return ratedDrivers.Count == 0
                ? 0
                : ratedDrivers.Average(d => d.Rating);
        }

        /// <summary>Returns all drivers flagged for low ratings requiring review.</summary>
        public List<Driver> GetLowRatedDrivers()
            => _unitOfWork.Drivers.GetLowRatedDrivers();

        /// <summary>Returns a summary of each driver's name and earnings, ordered by highest earner.</summary>
        public List<(string Name, decimal Earnings)> GetDriverEarningsSummary()
        {
            return _unitOfWork.Drivers
                .GetAll()
                .OrderByDescending(d => d.Earnings)
                .Select(d => (d.Name, d.Earnings))
                .ToList();
        }
    }
}
