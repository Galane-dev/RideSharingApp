using RideSharing.Application.Services;

namespace RideSharing.Presentation.ConsoleUI
{
    /// <summary>
    /// Displays platform-wide reports and moderation tools for the admin role.
    /// Admin credentials are hardcoded as this is a simulated console system.
    /// </summary>
    public class AdminMenu
    {
        private readonly ReportService _reportService;

        // Hardcoded admin credentials. A production system would store these securely.
        private const string AdminUsername = "admin";
        private const string AdminPassword = "admin123";

        public AdminMenu(ReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// Prompts for admin credentials and returns true if they match.
        /// </summary>
        public bool TryLogin(string username, string password)
            => username == AdminUsername && password == AdminPassword;

        /// <summary>
        /// Enters the admin menu loop.
        /// Returns when the admin chooses to log out.
        /// </summary>
        public void Show()
        {
            while (true)
            {
                ConsoleHelper.PrintHeader("Admin Menu");
                Console.WriteLine("1. View Platform Report");
                Console.WriteLine("2. View Driver Earnings");
                Console.WriteLine("3. View Low-Rated Drivers");
                Console.WriteLine("4. Logout");
                Console.Write("\nChoice: ");

                var choice = Console.ReadLine()?.Trim();
                Console.WriteLine();

                switch (choice)
                {
                    case "1": ShowPlatformReport(); break;
                    case "2": ShowDriverEarnings(); break;
                    case "3": ShowLowRatedDrivers(); break;
                    case "4": return;
                    default: ConsoleHelper.PrintError("Invalid choice. Please select 1–4."); break;
                }

                ConsoleHelper.Pause();
            }
        }

        #region Report Actions

        private void ShowPlatformReport()
        {
            ConsoleHelper.PrintHeader("Platform Report");

            Console.WriteLine($"  Total Completed Rides   : {_reportService.GetTotalCompletedRides()}");
            Console.WriteLine($"  Total Platform Earnings : {_reportService.GetTotalPlatformEarnings():C}");
            Console.WriteLine($"  Average Driver Rating   : {_reportService.GetAverageDriverRating():F2} / 5");
        }

        private void ShowDriverEarnings()
        {
            ConsoleHelper.PrintHeader("Driver Earnings Summary");

            var summary = _reportService.GetDriverEarningsSummary();

            if (summary.Count == 0)
            {
                Console.WriteLine("No driver earnings recorded yet.");
                return;
            }

            foreach (var (name, earnings) in summary)
                Console.WriteLine($"  {name,-20} {earnings,10:C}");
        }

        private void ShowLowRatedDrivers()
        {
            ConsoleHelper.PrintHeader("Flagged: Low-Rated Drivers");

            var drivers = _reportService.GetLowRatedDrivers();

            if (drivers.Count == 0)
            {
                ConsoleHelper.PrintSuccess("No drivers flagged for low ratings.");
                return;
            }

            foreach (var driver in drivers)
                Console.WriteLine($"  {driver.Name,-20} Rating: {driver.Rating:F1} / 5");
        }

        #endregion
    }
}
