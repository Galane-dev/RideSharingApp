using Ardalis.GuardClauses;
using RideSharing.Application.Interfaces;
using RideSharing.Domain.Constants;
using RideSharing.Domain.Entities;
using RideSharing.Domain.Enums;

namespace RideSharing.Application.Services
{
    /// <summary>
    /// Handles the submission and validation of driver ratings by passengers.
    /// </summary>
    public class RatingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RatingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Submits a star rating for the driver of a completed ride.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the ride is not completed or the passenger did not take this ride.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the rating is outside the allowed range.</exception>
        public void RateDriver(Guid passengerId, Guid rideId, int rating)
        {
            Guard.Against.Default(passengerId, nameof(passengerId));
            Guard.Against.Default(rideId, nameof(rideId));
            Guard.Against.OutOfRange(rating, nameof(rating), AppConstants.MinRating, AppConstants.MaxRating);

            var ride = GetCompletedRideOrThrow(rideId, passengerId);
            var driver = GetDriverOrThrow(ride.DriverId!.Value);

            driver.AddRating(rating);

            _unitOfWork.Drivers.Update(driver);
            _unitOfWork.Commit();
        }

        private Ride GetCompletedRideOrThrow(Guid rideId, Guid passengerId)
        {
            var ride = _unitOfWork.Rides.GetById(rideId)
                ?? throw new InvalidOperationException("Ride not found.");

            if (ride.PassengerId != passengerId)
                throw new InvalidOperationException("You can only rate rides you were a passenger on.");

            if (ride.Status != RideStatus.Completed)
                throw new InvalidOperationException("You can only rate completed rides.");

            if (ride.DriverId is null)
                throw new InvalidOperationException("This ride has no assigned driver to rate.");

            return ride;
        }

        private Driver GetDriverOrThrow(Guid driverId)
        {
            return _unitOfWork.Drivers.GetById(driverId)
                ?? throw new InvalidOperationException("Driver not found.");
        }
    }
}
