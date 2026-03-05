namespace RideSharing.Domain.Constants
{
    /// <summary>
    /// Centralised constants for pricing, ratings, and wallet defaults.
    /// Keeps magic numbers out of business logic.
    /// </summary>
    public static class AppConstants
    {
        /// <summary>Fare charged per kilometre of distance.</summary>
        public const decimal PricePerKm = 5m;

        /// <summary>Default starting balance applied to every new passenger wallet.</summary>
        public const decimal DefaultWalletBalance = 100m;

        /// <summary>Drivers whose average rating falls below this threshold are flagged for review.</summary>
        public const double LowRatingThreshold = 2.5;

        /// <summary>Minimum permissible star rating a passenger may submit.</summary>
        public const int MinRating = 1;

        /// <summary>Maximum permissible star rating a passenger may submit.</summary>
        public const int MaxRating = 5;

        /// <summary>Minimum amount that may be added to a passenger wallet in one transaction.</summary>
        public const decimal MinFundsAmount = 1m;
    }
}
