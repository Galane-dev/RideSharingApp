using RideSharing.Domain.Enums;

namespace RideSharing.Domain.Entities
{
    /// <summary>
    /// Represents a driver who can accept ride requests and accumulate earnings and ratings.
    /// </summary>
    public class Driver : User
    {
        public bool IsAvailable { get; set; } = true;

        public decimal Earnings { get; private set; }

        public double Rating { get; private set; }

        /// <summary>
        /// Tracks how many ratings have been submitted so the rolling average can be computed.
        /// </summary>
        private int _ratingCount;

        public Driver()
        {
            Role = UserRole.Driver;
        }

        /// <summary>
        /// Credits the driver's earnings after a ride is completed.
        /// </summary>
        public void AddEarnings(decimal amount)
        {
            Earnings += amount;
        }

        /// <summary>
        /// Incorporates a new star rating into the driver's rolling average.
        /// Uses incremental average formula to avoid storing all ratings.
        /// </summary>
        public void AddRating(int rating)
        {
            _ratingCount++;
            Rating = ((Rating * (_ratingCount - 1)) + rating) / _ratingCount;
        }
    }
}
