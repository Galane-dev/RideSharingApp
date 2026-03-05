using RideSharing.Application.Interfaces;
using RideSharing.Application.Services;
using RideSharing.Domain.Entities;

namespace RideSharing.Presentation.ConsoleUI
{
    /// <summary>
    /// Displays and handles all menu interactions available to a logged-in driver.
    /// </summary>
    public class DriverMenu
    {
        private readonly RideService _rideService;
        private readonly IUnitOfWork _unitOfWork;

        public DriverMenu(RideService rideService, IUnitOfWork unitOfWork)
        {
            _rideService = rideService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Enters the driver menu loop for the given driver session.
        /// Returns when the driver chooses to log out.
        /// </summary>
        public void Show(Driver driver)
        {
            while (true)
            {
                ConsoleHelper.PrintHeader($"Driver Menu — {driver.Name}");
                Console.WriteLine("1. View Available Ride Requests");
                Console.WriteLine("2. Accept a Ride");
                Console.WriteLine("3. Complete Current Ride");
                Console.WriteLine("4. View Earnings");
                Console.WriteLine("5. Toggle Availability");
                Console.WriteLine("6. Logout");
                Console.Write("\nChoice: ");

                var choice = Console.ReadLine()?.Trim();
                Console.WriteLine();

                switch (choice)
                {
                    case "1": ViewAvailableRequests(); break;
                    case "2": AcceptRide(driver); break;
                    case "3": CompleteRide(driver); break;
                    case "4": ViewEarnings(driver); break;
                    case "5": ToggleAvailability(driver); break;
                    case "6": return;
                    default: ConsoleHelper.PrintError("Invalid choice. Please select 1–6."); break;
                }

                ConsoleHelper.Pause();
            }
        }

        #region Menu Actions

        private void ViewAvailableRequests()
        {
            ConsoleHelper.PrintHeader("Available Ride Requests");

            var openRides = _rideService.GetOpenRequests();

            if (openRides.Count == 0)
            {
                Console.WriteLine("No ride requests available right now.");
                return;
            }

            foreach (var ride in openRides)
            {
                var passenger = _unitOfWork.Passengers.GetById(ride.PassengerId);
                Console.WriteLine($"  ID: {ride.Id}");
                Console.WriteLine($"     Passenger : {passenger?.Name ?? "Unknown"}");
                Console.WriteLine($"     Route     : {ride.PickupLocation} → {ride.DropoffLocation}");
                Console.WriteLine($"     Distance  : {ride.DistanceKm} km");
                Console.WriteLine($"     Fare      : {ride.Fare:C}");
                ConsoleHelper.PrintDivider();
            }
        }

        private void AcceptRide(Driver driver)
        {
            ConsoleHelper.PrintHeader("Accept a Ride");

            if (!driver.IsAvailable)
            {
                ConsoleHelper.PrintError("You are currently marked as unavailable. Toggle your availability first.");
                return;
            }

            var openRides = _rideService.GetOpenRequests();

            if (openRides.Count == 0)
            {
                Console.WriteLine("There are no open ride requests to accept.");
                return;
            }

            Console.WriteLine("Open requests:");

            for (var i = 0; i < openRides.Count; i++)
            {
                var ride = openRides[i];
                Console.WriteLine($"  {i + 1}. {ride.PickupLocation} → {ride.DropoffLocation} | {ride.DistanceKm} km | {ride.Fare:C}");
            }

            var index = ConsoleHelper.PromptInt("Select ride number", 1, openRides.Count) - 1;

            try
            {
                _rideService.AcceptRide(openRides[index].Id, driver.Id);

                // Refresh the local driver reference so availability reflects the change.
                var updatedDriver = _unitOfWork.Drivers.GetById(driver.Id)!;
                driver.IsAvailable = updatedDriver.IsAvailable;

                ConsoleHelper.PrintSuccess("Ride accepted! Head to the pickup location.");
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Could not accept ride: {ex.Message}");
            }
        }

        private void CompleteRide(Driver driver)
        {
            ConsoleHelper.PrintHeader("Complete Current Ride");

            var activeRide = _rideService.GetActiveRideForDriver(driver.Id);

            if (activeRide is null)
            {
                Console.WriteLine("You have no active ride to complete.");
                return;
            }

            Console.WriteLine($"  Route : {activeRide.PickupLocation} → {activeRide.DropoffLocation}");
            Console.WriteLine($"  Fare  : {activeRide.Fare:C}");

            try
            {
                _rideService.CompleteRide(activeRide.Id);

                // Refresh local driver so earnings and availability reflect the update.
                var updatedDriver = _unitOfWork.Drivers.GetById(driver.Id)!;
                driver.IsAvailable = updatedDriver.IsAvailable;

                ConsoleHelper.PrintSuccess($"Ride completed! Fare of {activeRide.Fare:C} has been credited to your earnings.");
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Could not complete ride: {ex.Message}");
            }
        }

        private static void ViewEarnings(Driver driver)
        {
            ConsoleHelper.PrintHeader("Your Earnings");
            Console.WriteLine($"  Total Earnings : {driver.Earnings:C}");
            Console.WriteLine($"  Rating         : {(driver.Rating > 0 ? $"{driver.Rating:F1} / 5" : "No ratings yet")}");
            Console.WriteLine($"  Available      : {(driver.IsAvailable ? "Yes" : "No")}");
        }

        private void ToggleAvailability(Driver driver)
        {
            driver.IsAvailable = !driver.IsAvailable;

            _unitOfWork.Drivers.Update(driver);
            _unitOfWork.Commit();

            var status = driver.IsAvailable ? "available" : "unavailable";
            ConsoleHelper.PrintSuccess($"You are now marked as {status}.");
        }

        #endregion
    }
}
