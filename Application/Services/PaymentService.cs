using RideSharing.Domain.Constants;
using RideSharing.Domain.Entities;

namespace RideSharing.Application.Services
{
    /// <summary>
    /// Handles fare calculation and wallet transactions between passengers and drivers.
    /// </summary>
    public class PaymentService
    {
        /// <summary>
        /// Calculates the ride fare based on distance travelled.
        /// </summary>
        public decimal CalculateFare(double distanceKm)
        {
            return (decimal)distanceKm * AppConstants.PricePerKm;
        }

        /// <summary>
        /// Transfers the fare from the passenger's wallet to the driver's earnings.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the passenger cannot afford the fare.</exception>
        public void ProcessPayment(Passenger passenger, Driver driver, decimal fare)
        {
            passenger.DeductFunds(fare);
            driver.AddEarnings(fare);
        }
    }
}
