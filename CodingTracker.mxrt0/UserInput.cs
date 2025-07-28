using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CodingTracker.mxrt0
{
    public class UserInput
    {
        private readonly MyCodingTrackerDatabase _db;
        public UserInput(MyCodingTrackerDatabase codingController)
        {
            _db = codingController;
        }

        public void MainMenu()
        {
            bool closeApp = false; 
            while (!closeApp)
            {
                AnsiConsole.MarkupLine($"[magenta2 bold][slowblink]\nMAIN MENU\n[/][/]");
                AnsiConsole.MarkupLine($"[magenta3_1]What would you like to do?\n[/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 0 to [italic][red bold]Close Application[/][/][/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 1 to [italic][mediumturquoise bold]View [orange3 bold]All [italic][mediumturquoise bold]Records[/][/][/][/][/][/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 2 to [italic][mediumturquoise bold]View Record[/][/][/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 3 to [italic][yellow2 bold]Add New [italic][mediumturquoise bold]Record[/][/][/][/][/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 4 to [italic][red bold]Delete [italic][mediumturquoise bold]Record[/][/][/][/][/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 5 to [italic][chartreuse2 bold]Update [italic][mediumturquoise bold]Record\n[/][/][/][/][/]");

                string userInput = Console.ReadLine();

                while (string.IsNullOrWhiteSpace(userInput))
                {
                    AnsiConsole.MarkupLine($"[italic][red]\nInvalid command. [yellow bold]Please enter a number [italic][red]from 0-4!\n[/][/][/][/][/]");
                    userInput = Console.ReadLine();
                }
                HandleUserInput(userInput, ref closeApp);
            }
        }
        public void HandleUserInput(string userInput, ref bool closeApp)
        {
            switch (userInput)
            {
                case "0":
                    AnsiConsole.MarkupLine("[green1 bold]\nGoodbye!\n[/]");
                    closeApp = true;
                    Environment.Exit(0);
                    break;
                case "1":
                    GetAll();
                    break;
                case "2":
                    ViewRecord();
                    break;
                case "3":
                    AddRecord();
                    break;
                case "4":
                    DeleteRecord();
                    break;
                case "5":
                    UpdateRecord();
                    break;
                default:
                    AnsiConsole.MarkupLine("[red][italic]\nInvalid command! [yellow bold]Enter number [italic][red]from 0-4.\n[/][/][/][/][/]");
                    break;
            }
        }

        private void ViewRecord()
        {
            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter the ID of the record you wish to view. Type 0 to return to main menu.\n[/][/]");

            string? idInput = Console.ReadLine();

            int id = Validation.ValidateId(idInput);

            var codingSession = _db.GetRecordById(id);

            while (codingSession.Id == 0)
            {
                AnsiConsole.MarkupLine($"[red][italic]\nRecord with ID [red bold]{id} does not exist.[/][/][/]");
                AnsiConsole.MarkupLine("[yellow bold]\nPlease enter a valid record ID or [magenta2]type 0 to return to Main Menu:\n[/][/]");

                idInput = Console.ReadLine();

                id = Validation.ValidateId(idInput);

                if (id == 0)
                {
                    MainMenu();
                    break;
                }

                codingSession = _db.GetRecordById(id);
            }

            AnsiConsole.MarkupLine($"[magenta2]\n{{ Id: {codingSession.Id}, Date: {codingSession.Date}, Start Time: {codingSession.StartTime}, End Time: {codingSession.EndTime} Duration: {codingSession.Duration} }}\n[/]");
        }

        private void UpdateRecord()
        {
            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter the ID of the record you wish to update. Type 0 to return to main menu.\n[/][/]");

            string? idInput = Console.ReadLine();

            int id = Validation.ValidateId(idInput);

            if (id == 0)
            {
                MainMenu();
            }

            var codingSession = _db.GetRecordById(id);

            while (codingSession.Id == 0)
            {
                AnsiConsole.MarkupLine($"[red][italic]\nRecord with ID [red bold]{id} does not exist.[/][/][/]");
                AnsiConsole.MarkupLine("[yellow bold]\nPlease enter a valid record ID or [magenta2]type 0 to return to Main Menu:\n[/][/]");

                idInput = Console.ReadLine();

                id = Validation.ValidateId(idInput);

                if (id == 0)
                {
                    MainMenu();
                    break;
                }

                codingSession = _db.GetRecordById(id);
            }

            var newDate = GetDateInput();

            var newStartTime = GetStartTimeInput();
            var newEndTime = GetEndTimeInput();

            var newDuration = CalculateDuration(ref newStartTime, ref newEndTime);

            _db.UpdateRecord(id, newDate, newStartTime, newEndTime, newDuration);

        }

        private void GetAll()
        {
            List<CodingSession> allRecords = _db.GetAllRecords();

            if (allRecords.Count == 0)
            {
                AnsiConsole.MarkupLine("[red bold]\nThe database is empty!\n[/]");   
            }
            else
            {
                foreach (var codingSession in allRecords)
                {
                    AnsiConsole.MarkupLine($"[magenta2]\n{{ Id: {codingSession.Id}, Date: {codingSession.Date}, Start Time: {codingSession.StartTime}, End Time: {codingSession.EndTime}, Duration: {codingSession.Duration} }}\n[/]");
                }
            }
       
        }

        private void DeleteRecord()
        {
            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter the ID of the record you wish to delete. Type 0 to return to main menu.\n[/][/]");

            string? idInput = Console.ReadLine();

            int id = Validation.ValidateId(idInput);

            if (id == 0)
            {
                MainMenu();
            }

            var codingSession = _db.GetRecordById(id);

            while (codingSession.Id == 0)
            {
                AnsiConsole.MarkupLine($"[red][italic]\nRecord with ID [red bold]{id} does not exist.[/][/][/]");
                AnsiConsole.MarkupLine("[yellow bold]\nPlease enter a valid record ID or [magenta2]type 0 to return to Main Menu:\n[/][/]");

                idInput = Console.ReadLine();

                id = Validation.ValidateId(idInput);

                if (id == 0)
                {
                    MainMenu();
                    break;
                }

                codingSession = _db.GetRecordById(id);
            }

            _db.DeleteRecord(codingSession.Id);
        }

        private void AddRecord()
        {
            string date = GetDateInput();

            string startTime = GetStartTimeInput();
            string endTime = GetEndTimeInput();

            string duration = CalculateDuration(ref startTime, ref endTime);
            _db.InsertNewRecord(new CodingSession(date, startTime, endTime, duration)); 
        }

        private string GetDateInput()
        {
            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter the date: (Format: dd-MM-yyyy). Type 0 to return to main menu.\n[/][/]");
            var userDateInput = Console.ReadLine();

            if (userDateInput == "0")
            {
                MainMenu();         
            }

            return Validation.ValidateDate(userDateInput);
        }

        private string GetStartTimeInput()
        {
            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter the start time: (Format: hh:mm). Type 0 to return to main menu.\n[/][/]");

            var startTimeInput = Console.ReadLine();

            if (startTimeInput == "0")
            {
                MainMenu();
            }

            return Validation.ValidateTime(startTimeInput);
        }

        private string GetEndTimeInput()
        {
            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter the end time: (Format: hh:mm). Type 0 to return to main menu.\n[/][/]");

            var endTimeInput = Console.ReadLine();

            if (endTimeInput == "0")
            {
                MainMenu();
            }

            return Validation.ValidateTime(endTimeInput);
        }
        private string CalculateDuration(ref string startTime, ref string endTime)
        {
            var end = TimeSpan.ParseExact(endTime, "h\\:mm", CultureInfo.InvariantCulture);
            var start = TimeSpan.ParseExact(startTime, "h\\:mm", CultureInfo.InvariantCulture);

            while (end < start)
            {
                AnsiConsole.MarkupLine("[red][italic]\nInvalid time range. End time must be after start time.\n[/][/]");
                startTime = GetStartTimeInput();
                endTime = GetEndTimeInput();
                end = TimeSpan.ParseExact(endTime, "h\\:mm", CultureInfo.InvariantCulture);
                start = TimeSpan.ParseExact(startTime, "h\\:mm", CultureInfo.InvariantCulture);
            }

            TimeSpan duration = end - start;
            return duration.ToString("hh\\:mm");
        }
    }
}
