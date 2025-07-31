using Spectre.Console;
using System.Globalization;

namespace CodingTracker.mxrt0
{
    public static class Validation
    {
        public static int ValidateId(string? idInput = "")
        {

            while (!int.TryParse(idInput, out _) || string.IsNullOrWhiteSpace(idInput) || int.Parse(idInput) < 0)
            {
                AnsiConsole.MarkupLine("[red][italic]\nInvalid ID! [yellow bold]Please enter a valid record ID or [magenta2]type 0 to return to Main Menu:\n[/][/][/][/]");
                idInput = Console.ReadLine();
                if (idInput == "0")
                {
                    return 0;
                }
            }

            int id = int.Parse(idInput);

            return id;
        }

        public static string ValidateDate(string? userDateInput = "")
        {

            while (!DateTime.TryParseExact(userDateInput.Trim(), "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                AnsiConsole.MarkupLine("[red][italic]\nInvalid date! [yellow bold]Please enter a valid date (Format: dd-MM-yyyy). [magenta2]Type 'Today' for current date or type 0 to return to Main Menu:\n[/][/][/][/]");
                userDateInput = Console.ReadLine();
                if (userDateInput == "0")
                {
                    return userDateInput;
                }
                if (userDateInput.Trim().ToLower() == "today")
                {
                    return DateTime.Today.ToString("dd-MM-yyyy");
                }
            }
            return userDateInput.Trim();
        }

        public static string ValidateTime(string? timeInput = "")
        {

            while (!TimeSpan.TryParseExact(timeInput.Trim(), "hh\\:mm", CultureInfo.InvariantCulture, out _))
            {
                AnsiConsole.MarkupLine("[red][italic]\nInvalid time! [yellow bold]Please enter a valid time (Format: hh:mm) or [magenta2]type 0 to return to Main Menu:\n[/][/][/][/]");
                timeInput = Console.ReadLine();
                if (timeInput == "0")
                {
                    return timeInput;
                }
            }
            return timeInput.Trim();
        }

        public static int ValidateYears(string? yearInput = "")
        {
            while (!int.TryParse(yearInput.Trim(), out _) || int.Parse(yearInput.Trim()) < 1)
            {
                AnsiConsole.MarkupLine($"[red][italic]\nInvalid year(s). [yellow bold]Please enter an integer greater than 0 or type 0 to return to Main Menu: \n[/][/][/]");
                yearInput = Console.ReadLine();
                if (yearInput == "0")
                {
                    return 0;
                }
            }
            return int.Parse(yearInput.Trim());
        }

        public static int ValidateWeeks(string? weeksInput = "")
        {
            while (!int.TryParse(weeksInput.Trim(), out _) || int.Parse(weeksInput.Trim()) < 1)
            {
                AnsiConsole.MarkupLine($"[red][italic]\nInvalid number of weeks. [yellow bold]Please enter an integer greater than 0 or type 0 to return to Main Menu: \n[/][/][/]");
                weeksInput = Console.ReadLine();
                if (weeksInput == "0")
                {
                    return 0;
                }
            }
            return int.Parse(weeksInput.Trim());
        }

        public static string ValidateFilter(string? filterInput = "")
        {

            while (!string.Equals(filterInput.Trim(), "days", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(filterInput.Trim(), "weeks", StringComparison.OrdinalIgnoreCase) && !string.Equals(filterInput.Trim(), "years", StringComparison.OrdinalIgnoreCase))
            {
                AnsiConsole.MarkupLine($"[red][italic]\nInvalid filter type. [yellow bold]Please enter a valid filter (days/weeks/years) or type 0 to return to Main Menu: \n[/][/][/]");
                filterInput = Console.ReadLine();
                if (filterInput == "0")
                {
                    return filterInput;
                }
            }
            return filterInput.Trim();
        }

        public static string ValidateOrder(string? orderInput = "")
        {

            while (!string.Equals(orderInput.Trim(), "ascending", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(orderInput.Trim(), "descending", StringComparison.OrdinalIgnoreCase))
            {
                AnsiConsole.MarkupLine($"[red][italic]\nInvalid order type. [yellow bold]Please enter a valid order type (ascending/descending) or type 0 to return to Main Menu: \n[/][/][/]");
                orderInput = Console.ReadLine();
                if (orderInput == "0")
                {
                    return orderInput;
                }
            }
            return orderInput.Trim();
        }

        public static string EnsureValidNewGoalName(string? goalNameInput = "")
        {

            while (string.IsNullOrEmpty(goalNameInput) || !goalNameInput.Trim().Any(char.IsLetter))
            {
                AnsiConsole.MarkupLine($"[red][italic]\nInvalid goal name. Please enter a non-null name containing at least 1 letter or type 0 to return to Main Menu: \n[/][/]");
                goalNameInput = Console.ReadLine();
                if (goalNameInput == "0")
                {
                    return goalNameInput;
                }
            }
            return goalNameInput.Trim();
        }

        public static void DisplayInvalidId(int id)
        {
            AnsiConsole.MarkupLine($"[red][italic]\nRecord with ID [red bold]{id} does not exist.[/][/][/]");
            AnsiConsole.MarkupLine("[yellow bold]\nPlease enter a valid record ID or [magenta2]type 0 to return to Main Menu:\n[/][/]");
        }
    }
}
