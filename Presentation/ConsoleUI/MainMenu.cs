using RideSharing.Application.Services;
using RideSharing.Domain.Enums;

namespace RideSharing.Presentation.ConsoleUI
{
    /// <summary>
    /// Entry point for the console UI. Handles registration, login,
    /// and routing authenticated users to their role-specific menus.
    /// </summary>
    public class MainMenu
    {
        private readonly AuthService _authService;
        private readonly PassengerMenu _passengerMenu;
        private readonly DriverMenu _driverMenu;
        private readonly AdminMenu _adminMenu;

        public MainMenu(
            AuthService authService,
            PassengerMenu passengerMenu,
            DriverMenu driverMenu,
            AdminMenu adminMenu)
        {
            _authService = authService;
            _passengerMenu = passengerMenu;
            _driverMenu = driverMenu;
            _adminMenu = adminMenu;
        }

        /// <summary>
        /// Starts the application loop. Runs until the user selects Exit.
        /// </summary>
        public void Start()
        {
            Console.WriteLine("\nWelcome to RideShare\n");

            while (true)
            {
                ConsoleHelper.PrintHeader("Main Menu");
                Console.WriteLine("1. Register as Passenger");
                Console.WriteLine("2. Register as Driver");
                Console.WriteLine("3. Login");
                Console.WriteLine("4. Exit");
                Console.Write("\nChoice: ");

                var choice = Console.ReadLine()?.Trim();
                Console.WriteLine();

                switch (choice)
                {
                    case "1": RegisterPassenger(); break;
                    case "2": RegisterDriver(); break;
                    case "3": Login(); break;
                    case "4":
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        ConsoleHelper.PrintError("Invalid choice. Please select 1–4.");
                        break;
                }

                ConsoleHelper.Pause();
            }
        }

        #region Registration

        private void RegisterPassenger()
        {
            ConsoleHelper.PrintHeader("Register as Passenger");

            var name = ConsoleHelper.PromptString("Full name");
            var username = ConsoleHelper.PromptString("Username");
            var password = ConsoleHelper.PromptPassword();

            try
            {
                var passenger = _authService.RegisterPassenger(name, username, password);
                ConsoleHelper.PrintSuccess($"Welcome, {passenger.Name}! Your wallet starts at {passenger.WalletBalance:C}.");
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Registration failed: {ex.Message}");
            }
        }

        private void RegisterDriver()
        {
            ConsoleHelper.PrintHeader("Register as Driver");

            var name = ConsoleHelper.PromptString("Full name");
            var username = ConsoleHelper.PromptString("Username");
            var password = ConsoleHelper.PromptPassword();

            try
            {
                var driver = _authService.RegisterDriver(name, username, password);
                ConsoleHelper.PrintSuccess($"Welcome, {driver.Name}! You are registered as a driver.");
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Registration failed: {ex.Message}");
            }
        }

        #endregion

        #region Login

        private void Login()
        {
            ConsoleHelper.PrintHeader("Login");

            var username = ConsoleHelper.PromptString("Username");
            var password = ConsoleHelper.PromptPassword();

            // Check admin credentials first, then passenger, then driver.
            if (_adminMenu.TryLogin(username, password))
            {
                _adminMenu.Show();
                return;
            }

            TryLoginAsPassenger(username, password);
        }

        private void TryLoginAsPassenger(string username, string password)
        {
            var passenger = _authService.LoginPassenger(username, password);

            if (passenger is not null)
            {
                _passengerMenu.Show(passenger);
                return;
            }

            TryLoginAsDriver(username, password);
        }

        private void TryLoginAsDriver(string username, string password)
        {
            var driver = _authService.LoginDriver(username, password);

            if (driver is not null)
            {
                _driverMenu.Show(driver);
                return;
            }

            ConsoleHelper.PrintError("Invalid username or password.");
        }

        #endregion
    }
}
