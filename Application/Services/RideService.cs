using Ardalis.GuardClauses;
using RideSharing.Application.Interfaces;
using RideSharing.Domain.Entities;
using RideSharing.Domain.Enums;

namespace RideSharing.Application.Services
{
    /// <summary>
    /// Orchestrates the full ride lifecycle: requesting, accepting, and completing rides.
    /// All state changes are committed through the Unit of Work.
    /// </summary>
    public class RideService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaymentService _paymentService;

        public RideService(IUnitOfWork unitOfWork, PaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }

        #region Ride Request

        /// <summary>
        /// Creates a new ride request for a passenger after validating their wallet balance.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the passenger cannot afford the fare.</exception>
        public Ride RequestRide(Guid passengerId, string pickup, string dropoff, double distanceKm)
        {
            Guard.Against.Default(passengerId, nameof(passengerId));
            Guard.Against.NullOrWhiteSpace(pickup, nameof(pickup));
            Guard.Against.NullOrWhiteSpace(dropoff, nameof(dropoff));
            Guard.Against.NegativeOrZero(distanceKm, nameof(distanceKm));

            // Reload ensures this instance sees rides and balances written by other running instances.
            _unitOfWork.Reload();

            var passenger = GetPassengerOrThrow(passengerId);
            var fare = _paymentService.CalculateFare(distanceKm);

            if (!passenger.CanAfford(fare))
                throw new InvalidOperationException(
                    $"Insufficient wallet balance. Fare: {fare:C}, Balance: {passenger.WalletBalance:C}.");

            var ride = BuildRide(passengerId, pickup, dropoff, distanceKm, fare);

            _unitOfWork.Rides.Add(ride);
            _unitOfWork.Commit();

            return ride;
        }

        private static Ride BuildRide(Guid passengerId, string pickup, string dropoff,
                                      double distanceKm, decimal fare)
        {
            return new Ride
            {
                PassengerId = passengerId,
                PickupLocation = pickup,
                DropoffLocation = dropoff,
                DistanceKm = distanceKm,
                Fare = fare,
                Status = RideStatus.Requested
            };
        }

        #endregion

        #region Accept Ride

        /// <summary>
        /// Assigns an available driver to an open ride request.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when ride is not open or driver is unavailable.</exception>
        public void AcceptRide(Guid rideId, Guid driverId)
        {
            Guard.Against.Default(rideId, nameof(rideId));
            Guard.Against.Default(driverId, nameof(driverId));

            // Reload so this driver sees ride requests posted by passenger instances.
            _unitOfWork.Reload();

            var ride = GetRideOrThrow(rideId);
            var driver = GetDriverOrThrow(driverId);

            EnsureRideIsOpen(ride);
            EnsureDriverIsAvailable(driver);

            ride.DriverId = driverId;
            ride.Status = RideStatus.Accepted;
            driver.IsAvailable = false;

            _unitOfWork.Rides.Update(ride);
            _unitOfWork.Drivers.Update(driver);
            _unitOfWork.Commit();
        }

        private static void EnsureRideIsOpen(Ride ride)
        {
            if (ride.Status != RideStatus.Requested)
                throw new InvalidOperationException("This ride is no longer available.");
        }

        private static void EnsureDriverIsAvailable(Driver driver)
        {
            if (!driver.IsAvailable)
                throw new InvalidOperationException("Driver is currently unavailable.");
        }

        #endregion

        #region Complete Ride

        /// <summary>
        /// Marks a ride as completed and processes the fare payment.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the ride is not in an Accepted state.</exception>
        public void CompleteRide(Guid rideId)
        {
            Guard.Against.Default(rideId, nameof(rideId));

            // Reload so the driver instance sees the accepted ride written by another instance.
            _unitOfWork.Reload();

            var ride = GetRideOrThrow(rideId);

            if (ride.Status != RideStatus.Accepted)
                throw new InvalidOperationException("Only an accepted ride can be completed.");

            var passenger = GetPassengerOrThrow(ride.PassengerId);
            var driver = GetDriverOrThrow(ride.DriverId!.Value);

            _paymentService.ProcessPayment(passenger, driver, ride.Fare);

            ride.Status = RideStatus.Completed;
            ride.CompletedAt = DateTime.UtcNow;
            driver.IsAvailable = true;

            _unitOfWork.Rides.Update(ride);
            _unitOfWork.Passengers.Update(passenger);
            _unitOfWork.Drivers.Update(driver);
            _unitOfWork.Commit();
        }

        #endregion

        #region Queries

        /// <summary>Returns all rides currently awaiting a driver.</summary>
        public List<Ride> GetOpenRequests()
        {
            _unitOfWork.Reload();
            return _unitOfWork.Rides.GetOpenRequests();
        }

        /// <summary>Returns the ride history for a specific passenger.</summary>
        public List<Ride> GetRideHistoryForPassenger(Guid passengerId)
        {
            _unitOfWork.Reload();
            return _unitOfWork.Rides.GetRidesForPassenger(passengerId);
        }

        /// <summary>Returns all available drivers using LINQ filtering.</summary>
        public List<Driver> GetAvailableDrivers()
        {
            _unitOfWork.Reload();
            return _unitOfWork.Drivers.GetAvailableDrivers();
        }

        /// <summary>Returns the active ride for a driver, or null if none.</summary>
        public Ride? GetActiveRideForDriver(Guid driverId)
        {
            _unitOfWork.Reload();
            return _unitOfWork.Rides.GetActiveRideForDriver(driverId);
        }

        #endregion

        #region Private Helpers

        private Passenger GetPassengerOrThrow(Guid id)
        {
            return _unitOfWork.Passengers.GetById(id)
                ?? throw new InvalidOperationException("Passenger not found.");
        }

        private Driver GetDriverOrThrow(Guid id)
        {
            return _unitOfWork.Drivers.GetById(id)
                ?? throw new InvalidOperationException("Driver not found.");
        }

        private Ride GetRideOrThrow(Guid id)
        {
            return _unitOfWork.Rides.GetById(id)
                ?? throw new InvalidOperationException("Ride not found.");
        }

        #endregion
    }
}
