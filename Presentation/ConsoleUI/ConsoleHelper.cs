namespace RideSharing.Presentation.ConsoleUI
{
    /// <summary>
    /// Shared console display utilities used across all menus.
    /// Centralises formatting to keep individual menus free of repetitive write logic.
    /// </summary>
    public static class ConsoleHelper
    {
        /// <summary>Writes a full-width divider line to the console.</summary>
        public static void PrintDivider()
            => Console.WriteLine(new string('-', 50));

        /// <summary>Prints a header block with a title centred between dividers.</summary>
        public static void PrintHeader(string title)
        {
            PrintDivider();
            Console.WriteLine($"  {title}");
            PrintDivider();
        }

        /// <summary>Prints a success message in green.</summary>
        public static void PrintSuccess(string message)
            => PrintColoured(message, ConsoleColor.Green);

        /// <summary>Prints an error message in red.</summary>
        public static void PrintError(string message)
            => PrintColoured(message, ConsoleColor.Red);

        /// <summary>Prompts the user for a string value. Repeats until input is non-empty.</summary>
        public static string PromptString(string label)
        {
            while (true)
            {
                Console.Write($"{label}: ");
                var input = Console.ReadLine()?.Trim();

                if (!string.IsNullOrWhiteSpace(input))
                    return input;

                PrintError("Input cannot be empty. Please try again.");
            }
        }

        /// <summary>Prompts the user for a password, masking input with asterisks.</summary>
        public static string PromptPassword(string label = "Password")
        {
            Console.Write($"{label}: ");
            var password = string.Empty;

            while (true)
            {
                var key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Enter)
                    break;

                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password[..^1];
                    Console.Write("\b \b");
                    continue;
                }

                password += key.KeyChar;
                Console.Write('*');
            }

            Console.WriteLine();
            return password;
        }

        /// <summary>
        /// Prompts the user for a positive decimal value.
        /// Repeats until valid input is entered.
        /// </summary>
        public static decimal PromptDecimal(string label)
        {
            while (true)
            {
                Console.Write($"{label}: ");

                if (decimal.TryParse(Console.ReadLine(), out var value) && value > 0)
                    return value;

                PrintError("Please enter a valid positive number.");
            }
        }

        /// <summary>
        /// Prompts the user for a positive double value.
        /// Repeats until valid input is entered.
        /// </summary>
        public static double PromptDouble(string label)
        {
            while (true)
            {
                Console.Write($"{label}: ");

                if (double.TryParse(Console.ReadLine(), out var value) && value > 0)
                    return value;

                PrintError("Please enter a valid positive number.");
            }
        }

        /// <summary>
        /// Prompts the user for an integer within the given inclusive range.
        /// Repeats until valid input is entered.
        /// </summary>
        public static int PromptInt(string label, int min, int max)
        {
            while (true)
            {
                Console.Write($"{label} ({min}-{max}): ");

                if (int.TryParse(Console.ReadLine(), out var value) && value >= min && value <= max)
                    return value;

                PrintError($"Please enter a number between {min} and {max}.");
            }
        }

        /// <summary>Pauses execution until the user presses Enter.</summary>
        public static void Pause()
        {
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        private static void PrintColoured(string message, ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
