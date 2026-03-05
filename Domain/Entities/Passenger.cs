using RideSharing.Domain.Constants;
using RideSharing.Domain.Enums;

namespace RideSharing.Domain.Entities
{
    /// <summary>
    /// Represents a passenger who can request rides and manage a virtual wallet.
    /// </summary>
    public class Passenger : User
    {
        public decimal WalletBalance { get; private set; }

        public Passenger()
        {
            Role = UserRole.Passenger;
            WalletBalance = AppConstants.DefaultWalletBalance;
        }

        /// <summary>
        /// Adds the specified amount to the passenger's wallet balance.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when amount is below the minimum.</exception>
        public void AddFunds(decimal amount)
        {
            if (amount < AppConstants.MinFundsAmount)
                throw new ArgumentException($"Amount must be at least {AppConstants.MinFundsAmount:C}.");

            WalletBalance += amount;
        }

        /// <summary>
        /// Deducts the specified fare from the passenger's wallet.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when balance is insufficient.</exception>
        public void DeductFunds(decimal amount)
        {
            if (WalletBalance < amount)
                throw new InvalidOperationException(
                    $"Insufficient balance. Required: {amount:C}, Available: {WalletBalance:C}.");

            WalletBalance -= amount;
        }

        /// <summary>
        /// Returns true when the passenger can afford the given fare.
        /// Used as a pre-check before creating a ride request.
        /// </summary>
        public bool CanAfford(decimal fare) => WalletBalance >= fare;
    }
}
