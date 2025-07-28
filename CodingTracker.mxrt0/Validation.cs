using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static string ValidateDate(string userDateInput) 
        {
            while (!DateTime.TryParseExact(userDateInput, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                AnsiConsole.MarkupLine("[red][italic]\nInvalid date! [yellow bold]Please enter a valid date (Format: dd-MM-yyyy) or [magenta2]type 0 to return to Main Menu:\n[/][/][/][/]");
                userDateInput = Console.ReadLine();
                if (userDateInput == "0")
                {
                    break;
                }
            }
            return userDateInput;
        }

        public static string ValidateTime(string timeInput)
        {
            while (!TimeSpan.TryParseExact(timeInput, "h\\:mm", CultureInfo.InvariantCulture, out _))
            {
                AnsiConsole.MarkupLine("[red][italic]\n Invalid time! [yellow bold]Please enter a valid time (Format: hh:mm) or [magenta2]type 0 to return to Main Menu:\n[/][/][/][/]");
                timeInput = Console.ReadLine();
                if (timeInput == "0")
                {
                    break;
                }
            }
            return timeInput;
        }
    }
}
