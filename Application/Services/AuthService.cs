using Ardalis.GuardClauses;
using RideSharing.Application.Interfaces;
using RideSharing.Domain.Constants;
using RideSharing.Domain.Entities;

namespace RideSharing.Application.Services
{
    /// <summary>
    /// Handles user registration and credential-based login for passengers and drivers.
    /// </summary>
    public class AuthService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Registers a new passenger and seeds their wallet with the default balance.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the username is already taken.</exception>
        public Passenger RegisterPassenger(string name, string username, string password)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Guard.Against.NullOrWhiteSpace(username, nameof(username));
            Guard.Against.NullOrWhiteSpace(password, nameof(password));

            EnsureUsernameIsUnique(username);

            var passenger = new Passenger
            {
                Name = name,
                Username = username,
                Password = password
            };

            // Wallet is seeded with default balance inside the Passenger constructor.
            // We call AddFunds here only if additional seeding beyond the constructor default is needed.
            // The constructor already applies AppConstants.DefaultWalletBalance.

            _unitOfWork.Passengers.Add(passenger);
            _unitOfWork.Commit();

            return passenger;
        }

        /// <summary>
        /// Registers a new driver and sets them as available by default.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the username is already taken.</exception>
        public Driver RegisterDriver(string name, string username, string password)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Guard.Against.NullOrWhiteSpace(username, nameof(username));
            Guard.Against.NullOrWhiteSpace(password, nameof(password));

            EnsureUsernameIsUnique(username);

            var driver = new Driver
            {
                Name = name,
                Username = username,
                Password = password
            };

            _unitOfWork.Drivers.Add(driver);
            _unitOfWork.Commit();

            return driver;
        }

        /// <summary>
        /// Attempts to log in as a passenger using username and password.
        /// Returns the passenger on success, or null on failure.
        /// </summary>
        public Passenger? LoginPassenger(string username, string password)
        {
            var passenger = _unitOfWork.Passengers.GetByUsername(username);

            return passenger?.Password == password ? passenger : null;
        }

        /// <summary>
        /// Attempts to log in as a driver using username and password.
        /// Returns the driver on success, or null on failure.
        /// </summary>
        public Driver? LoginDriver(string username, string password)
        {
            var driver = _unitOfWork.Drivers.GetByUsername(username);

            return driver?.Password == password ? driver : null;
        }

        /// <summary>
        /// Checks that no existing passenger or driver already holds the given username.
        /// Usernames are shared across both roles to prevent ambiguity at login.
        /// </summary>
        private void EnsureUsernameIsUnique(string username)
        {
            var takenByPassenger = _unitOfWork.Passengers.GetByUsername(username) is not null;
            var takenByDriver = _unitOfWork.Drivers.GetByUsername(username) is not null;

            if (takenByPassenger || takenByDriver)
                throw new InvalidOperationException($"Username '{username}' is already taken.");
        }
    }
}
