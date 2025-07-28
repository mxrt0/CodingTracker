using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CodingTracker.mxrt0
{
    public class UserInput
    {
        private readonly MyCodingTrackerDatabase _db;
        private readonly string InvalidCommandMessage = "[italic][red]\nInvalid command. [yellow bold]Please enter a [italic]number [italic][red]from 0-5!\n[/][/][/][/][/][/]";
        private readonly string EnterIdMessage = "[magenta2][slowblink]\nPlease enter the ID of the record you wish to view. Type 0 to return to main menu.\n[/][/]";

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
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 2 to [italic][yellow bold]Filter [italic][mediumturquoise bold]Records[/][/][/][/][/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 3 to [italic][mediumturquoise bold]View Record[/][/][/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 4 to [italic][yellow2 bold]Add New [italic][mediumturquoise bold]Record[/][/][/][/][/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 5 to [italic][red bold]Delete [italic][mediumturquoise bold]Record[/][/][/][/][/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 6 to [italic][chartreuse2 bold]Update [italic][mediumturquoise bold]Record\n[/][/][/][/][/]");

                string? userInput = Console.ReadLine();

                while (string.IsNullOrWhiteSpace(userInput))
                {
                    AnsiConsole.MarkupLine(InvalidCommandMessage);
                    userInput = Console.ReadLine();
                }
                HandleUserInput(userInput, ref closeApp);
            }
            Environment.Exit(0);
        }
        public void HandleUserInput(string userInput, ref bool closeApp)
        {
            switch (userInput)
            {
                case "0":
                    AnsiConsole.MarkupLine("[green1 bold]\nGoodbye!\n[/]");
                    closeApp = true;
                    break;
                case "1":
                    GetAll();
                    break;
                case "2":
                    Filter();
                    break;
                case "3":
                    ViewRecord();
                    break;
                case "4":
                    AddRecord();
                    break;
                case "5":
                    DeleteRecord();
                    break;
                case "6":
                    UpdateRecord();
                    break;
                default:
                    AnsiConsole.MarkupLine(InvalidCommandMessage);
                    break;
            }
        }

        private void Filter()
        {
            List<CodingSession> allRecords = _db.GetAllRecords();
            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease select filter type: (days/weeks/years). Type 0 to return to main menu.\n[/][/]");
            string filterType = Console.ReadLine();
            List<CodingSession> filteredRecords;
            switch (filterType)
            {
                case "days":
                    AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease select starting date: (Format: dd-MM-yyyy): \n[/][/]");
                    string? startDateInput = Console.ReadLine();
                    DateTime startDate = DateTime.ParseExact(Validation.ValidateDate(startDateInput), "dd-MM-yyyy", CultureInfo.InvariantCulture);

                    AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease select end date: (Format: dd-MM-yyyy) or type 1 to only filter by previous date: \n[/][/]");
                    string? endDateInput = Console.ReadLine();
                    DateTime endDate = (endDateInput == "1") ? startDate 
                        : DateTime.ParseExact(Validation.ValidateDate(endDateInput), "dd-MM-yyyy", CultureInfo.InvariantCulture);

                    filteredRecords = allRecords.Where(r =>
                    {
                        DateTime recordDate = DateTime.ParseExact(r.Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        return recordDate >= startDate && recordDate <= endDate;
                    }).ToList();

                    Table table = new Table();
                    foreach (var property in typeof(CodingSession).GetProperties())
                    {
                        table.AddColumn(property.Name.ToString(), c => c.Centered());
                    }
                    foreach (var codingSession in filteredRecords)
                    {
                        table.AddRow(new string[] { codingSession.Id.ToString(), codingSession.Date, codingSession.StartTime, codingSession.EndTime, codingSession.Duration });
                    }
                    table.Border(TableBorder.Rounded);
                    table.ShowRowSeparators();
                    AnsiConsole.Write(table);
                    break;

            }
            
        }
        private void ViewRecord()
        {
            AnsiConsole.MarkupLine(EnterIdMessage);

            string? idInput = Console.ReadLine();

            int id = Validation.ValidateId(idInput);

            var codingSession = _db.GetRecordById(id);

            while (codingSession.Id == 0)
            {
                Validation.InvalidId(id);

                idInput = Console.ReadLine();

                id = Validation.ValidateId(idInput);

                if (id == 0)
                {
                    MainMenu();
                    break;
                }

                codingSession = _db.GetRecordById(id);
            }
            Table singleItemTable = new Table();
            foreach (var property in codingSession.GetType().GetProperties())
            {
                singleItemTable.AddColumn(property.Name.ToString(), c => c.Centered());
            }
            singleItemTable.AddRow(new string[] { codingSession.Id.ToString(), codingSession.Date, codingSession.StartTime, codingSession.EndTime, codingSession.Duration });
            AnsiConsole.Write(singleItemTable); 
        }

        private void UpdateRecord()
        {
            AnsiConsole.MarkupLine(EnterIdMessage);

            string? idInput = Console.ReadLine();

            int id = Validation.ValidateId(idInput);

            if (id == 0)
            {
                MainMenu();
            }

            var codingSession = _db.GetRecordById(id);

            while (codingSession.Id == 0)
            {
                Validation.InvalidId(id); ;

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
                Table table = new Table();
                foreach (var property in typeof(CodingSession).GetProperties())
                {
                    table.AddColumn(property.Name.ToString(), c => c.Centered());
                }
                foreach (var codingSession in allRecords)
                {
                    table.AddRow(new string[] { codingSession.Id.ToString(), codingSession.Date, codingSession.StartTime, codingSession.EndTime, codingSession.Duration });
                }
                table.Border(TableBorder.Rounded);
                table.ShowRowSeparators();
                AnsiConsole.Write(table);
            }
        }

        private void DeleteRecord()
        {
            AnsiConsole.MarkupLine(EnterIdMessage);

            string? idInput = Console.ReadLine();

            int id = Validation.ValidateId(idInput);

            if (id == 0)
            {
                MainMenu();
            }

            var codingSession = _db.GetRecordById(id);

            while (codingSession.Id == 0)
            {
                Validation.InvalidId(id);

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
            if (startTime == "Track time")
            {
                (string Start, string End) times = TrackTime();
                string duration = CalculateDuration(ref times.Start, ref times.End);
                _db.InsertNewRecord(new CodingSession(date, times.Start, times.End, duration));
            }
            else
            {
                string endTime = GetEndTimeInput();
                string duration = CalculateDuration(ref startTime, ref endTime);
                _db.InsertNewRecord(new CodingSession(date, startTime, endTime, duration));
            }       

        }

        private string GetDateInput()
        {
            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter the date: (Format: dd-MM-yyyy). Type 0 to return to main menu.\n[/][/]");
            string? userDateInput = Console.ReadLine();

            if (userDateInput == "0")
            {
                MainMenu();         
            }

            return Validation.ValidateDate(userDateInput);
        }

        private string GetStartTimeInput()
        {
            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter the start time (Format: hh:mm) or type 1 to track from current time. Type 0 to return to main menu.\n[/][/]");

            string? startTimeInput = Console.ReadLine();

            if (startTimeInput == "0")
            {
                MainMenu();
            }

            if (startTimeInput == "1")
            {
                return "Track time";
            }

            return Validation.ValidateTime(startTimeInput);
        }

        private (string startTime, string endTime) TrackTime()
        {
            var startTime = DateTime.Now.ToString("hh\\:mm");
            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nTime tracking has started. Type 0 to abandon and return to Main Menu or type 1 to end current coding session:\n[/][/]");

            var input = Console.ReadLine();

            if (input == "0")
            {
                MainMenu();   
            }
           
            var endTime = DateTime.Now.ToString("hh\\:mm");

            return (startTime, endTime);
    
        }

        private string GetEndTimeInput(bool tracking = false)
        {
            if (tracking)
            {
                var input = Console.ReadLine();

                if (input == "0")
                {
                    MainMenu();
                }

                return DateTime.Now.ToString("hh\\:mm"); 
            }

            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter the end time: (Format: hh:mm). Type 0 to return to main menu.\n[/][/]");

            string? endTimeInput = Console.ReadLine();

            if (endTimeInput == "0")
            {
                MainMenu();
            }

            return Validation.ValidateTime(endTimeInput);
        }
        private string CalculateDuration(ref string startTime, ref string endTime)
        {
            
            var start = TimeSpan.ParseExact(startTime, "h\\:mm", CultureInfo.InvariantCulture);
            var end = TimeSpan.ParseExact(endTime, "h\\:mm", CultureInfo.InvariantCulture);
       
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
