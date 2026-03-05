using RideSharing.Application.Interfaces;
using RideSharing.Application.Services;
using RideSharing.Domain.Constants;
using RideSharing.Domain.Entities;
using RideSharing.Domain.Enums;

namespace RideSharing.Presentation.ConsoleUI
{
    /// <summary>
    /// Displays and handles all menu interactions available to a logged-in passenger.
    /// </summary>
    public class PassengerMenu
    {
        private readonly RideService _rideService;
        private readonly RatingService _ratingService;
        private readonly IUnitOfWork _unitOfWork;

        public PassengerMenu(RideService rideService, RatingService ratingService, IUnitOfWork unitOfWork)
        {
            _rideService = rideService;
            _ratingService = ratingService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Enters the passenger menu loop for the given passenger session.
        /// Returns when the passenger chooses to log out.
        /// </summary>
        public void Show(Passenger passenger)
        {
            while (true)
            {
                ConsoleHelper.PrintHeader($"Passenger Menu — {passenger.Name}");
                Console.WriteLine("1. Request a Ride");
                Console.WriteLine("2. View Wallet Balance");
                Console.WriteLine("3. Add Funds to Wallet");
                Console.WriteLine("4. View Ride History");
                Console.WriteLine("5. Rate a Driver");
                Console.WriteLine("6. Logout");
                Console.Write("\nChoice: ");

                var choice = Console.ReadLine()?.Trim();
                Console.WriteLine();

                switch (choice)
                {
                    case "1": RequestRide(passenger); break;
                    case "2": ViewWalletBalance(passenger); break;
                    case "3": AddFunds(passenger); break;
                    case "4": ViewRideHistory(passenger); break;
                    case "5": RateDriver(passenger); break;
                    case "6": return;
                    default: ConsoleHelper.PrintError("Invalid choice. Please select 1–6."); break;
                }

                ConsoleHelper.Pause();
            }
        }

        #region Menu Actions

        private void RequestRide(Passenger passenger)
        {
            ConsoleHelper.PrintHeader("Request a Ride");

            var pickup = ConsoleHelper.PromptString("Pickup location");
            var dropoff = ConsoleHelper.PromptString("Drop-off location");
            var distance = ConsoleHelper.PromptDouble("Distance (km)");

            try
            {
                var ride = _rideService.RequestRide(passenger.Id, pickup, dropoff, distance);
                ConsoleHelper.PrintSuccess($"Ride requested! Fare: {ride.Fare:C} | Ride ID: {ride.Id}");
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Could not request ride: {ex.Message}");
            }
        }

        private static void ViewWalletBalance(Passenger passenger)
        {
            // Re-read the passenger to reflect any balance changes made this session.
            ConsoleHelper.PrintHeader("Wallet Balance");
            Console.WriteLine($"  Current Balance: {passenger.WalletBalance:C}");
        }

        private void AddFunds(Passenger passenger)
        {
            ConsoleHelper.PrintHeader("Add Funds");

            var amount = ConsoleHelper.PromptDecimal("Amount to add");

            try
            {
                passenger.AddFunds(amount);
                _unitOfWork.Passengers.Update(passenger);
                _unitOfWork.Commit();

                ConsoleHelper.PrintSuccess($"Added {amount:C}. New balance: {passenger.WalletBalance:C}");
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Could not add funds: {ex.Message}");
            }
        }

        private void ViewRideHistory(Passenger passenger)
        {
            ConsoleHelper.PrintHeader("Ride History");

            var rides = _rideService.GetRideHistoryForPassenger(passenger.Id);

            if (rides.Count == 0)
            {
                Console.WriteLine("You have no ride history yet.");
                return;
            }

            foreach (var ride in rides)
                PrintRideSummary(ride);
        }

        private void RateDriver(Passenger passenger)
        {
            ConsoleHelper.PrintHeader("Rate a Driver");

            var completedRides = _rideService
                .GetRideHistoryForPassenger(passenger.Id)
                .Where(r => r.Status == RideStatus.Completed && r.DriverId.HasValue)
                .ToList();

            if (completedRides.Count == 0)
            {
                Console.WriteLine("You have no completed rides available to rate.");
                return;
            }

            Console.WriteLine("Your completed rides:");

            for (var i = 0; i < completedRides.Count; i++)
            {
                var ride = completedRides[i];
                var driver = _unitOfWork.Drivers.GetById(ride.DriverId!.Value);
                Console.WriteLine($"  {i + 1}. {ride.PickupLocation} → {ride.DropoffLocation} | Driver: {driver?.Name ?? "Unknown"} | {ride.CompletedAt:dd MMM yyyy}");
            }

            var rideIndex = ConsoleHelper.PromptInt("Select ride number", 1, completedRides.Count) - 1;
            var rating = ConsoleHelper.PromptInt("Star rating", AppConstants.MinRating, AppConstants.MaxRating);

            try
            {
                _ratingService.RateDriver(passenger.Id, completedRides[rideIndex].Id, rating);
                ConsoleHelper.PrintSuccess("Rating submitted. Thank you!");
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Could not submit rating: {ex.Message}");
            }
        }

        #endregion

        #region Display Helpers

        private static void PrintRideSummary(Ride ride)
        {
            Console.WriteLine($"  [{ride.Status}] {ride.PickupLocation} → {ride.DropoffLocation} | {ride.DistanceKm} km | Fare: {ride.Fare:C} | {ride.RequestedAt:dd MMM yyyy}");
        }

        #endregion
    }
}
