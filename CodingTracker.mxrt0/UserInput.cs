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
        private readonly string InvalidCommandMessage = "[italic][red]\nInvalid command. [yellow bold]Please enter a [italic]number [italic][red]from 0-11!\n[/][/][/][/][/][/]";
        private readonly string EnterIdMessage = "[magenta2][slowblink]\nPlease enter the ID of the record you wish to view. Type 0 to return to main menu.\n[/][/]";
        private readonly GoalManager _goalManager;

        public UserInput(MyCodingTrackerDatabase codingController)
        {
            _db = codingController;
            _goalManager = new GoalManager();
        }

        public void MainMenu()
        {
            bool closeApp = false; 
            while (!closeApp)
            {
                AnsiConsole.MarkupLine($"[magenta2 bold][slowblink]\nMAIN MENU\n[/][/]");
                AnsiConsole.MarkupLine($"[magenta3_1]What would you like to do?\n[/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 0 to [italic][red bold]Close Application[/][/][/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 1 to [italic][orange3 bold]View All Records[/][/][/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 2 to [italic][yellow bold]Filter Records[/][/][/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 3 to [italic][mediumturquoise bold]View Record[/][/][/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 4 to [italic][yellow2 bold]Add New Record[/][/][/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 5 to [italic][red bold]Delete Record[/][/][/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 6 to [italic][chartreuse2 bold]Update Record[/][/][/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 7 to [italic][yellow bold]View Report By Period[/][/][/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 8 to [italic][chartreuse2 bold]Set New Coding Goal[/][/][/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 9 to [italic][red bold]Delete Coding Goal[/][/][/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 10 to [italic][magenta bold]Check Coding Goal Progress[/][/][/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 11 to [italic][yellow bold]Update Coding Goal Progress\n[/][/][/]");

                string? userInput = Console.ReadLine();

                while (string.IsNullOrWhiteSpace(userInput))
                {
                    AnsiConsole.MarkupLine(InvalidCommandMessage);
                    userInput = Console.ReadLine();   
                }

                AnsiConsole.Clear();
                HandleUserInput(userInput);
            }   
        }
        public void HandleUserInput(string userInput)
        {
            switch (userInput)
            {
                case "0":
                    AnsiConsole.MarkupLine("[green1 bold]\nGoodbye!\n[/]");
                    Environment.Exit(0);
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
                case "7":
                    ShowReport();
                    break;
                case "8":
                    SetNewGoal();
                    break;
                case "9":
                    DeleteGoal();
                    break;
                    case "10":
                    CheckGoalProgress();
                    break;
                case "11":
                    UpdateGoalProgress();
                    break;
                default:
                    AnsiConsole.MarkupLine(InvalidCommandMessage);
                    break;
            }
        }

        private void UpdateGoalProgress()
        {
            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter the exact name of your goal. Type 0 to return to main menu.\n[/][/]");
            string? goalToUpdateName = Console.ReadLine();
            if (goalToUpdateName == "0")
            {
                MainMenu();
            }

            while (string.IsNullOrEmpty(goalToUpdateName) || !_goalManager.ValidateGoalExists(goalToUpdateName))
            {
                AnsiConsole.MarkupLine("[red][italic]\nNo coding goal with this name was found. Try again or type 0 to return to Main Menu: \n[/][/]");
                goalToUpdateName = Console.ReadLine();
                if (goalToUpdateName == "0")
                {
                    MainMenu();
                }
            }

            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nEnter the hours/minutes of coding you have done toward your goal (Format: hh:mm). Type 0 to return to main menu.\n[/][/]");
            string codingContributionTimeInput = Console.ReadLine();
            string codingContributionTime = Validation.ValidateTime(codingContributionTimeInput);
            if (codingContributionTime == "0")
            {
                MainMenu();
            }

            var updatedGoal = _goalManager.UpdateGoal(goalToUpdateName, codingContributionTime);
            _goalManager.DisplayGoalProgress(updatedGoal.Name);

            if (updatedGoal.IsCompleted)
            {
                AnsiConsole.MarkupLine($"[green bold]Congratulations, you have reached goal '{updatedGoal.Name}'![/]");
                _goalManager.DeleteGoal(updatedGoal.Name);
            }
            else
            {
                var remainingTime = updatedGoal.TimeTarget - updatedGoal.CompletedTime;
                AnsiConsole.MarkupLine($"[green bold]Keep coding, you need {remainingTime.ToString("hh\\:mm")} more hour(s)/minute(s) to reach this goal![/]");
            }    
      
        }

        private void CheckGoalProgress()
        {
            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter the exact name of your goal. Type 0 to return to main menu.\n[/][/]");
            string? goalToCheckNameInput = Console.ReadLine();
            if (goalToCheckNameInput == "0")
            {
                MainMenu();
            }

            while (string.IsNullOrEmpty(goalToCheckNameInput) || !_goalManager.ValidateGoalExists(goalToCheckNameInput))
            {
                AnsiConsole.MarkupLine("[red][italic]\nNo coding goal with this name was found. Try again or type 0 to return to Main Menu: \n[/][/]");
                goalToCheckNameInput = Console.ReadLine();
                if (goalToCheckNameInput == "0")
                {
                    MainMenu();
                }
            }

            _goalManager.DisplayGoalProgress(goalToCheckNameInput);
        }

        private void DeleteGoal()
        {
            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter the exact name of your goal. Type 0 to return to main menu.\n[/][/]");
            string? goalToDeleteNameInput = Console.ReadLine();

            while (string.IsNullOrEmpty(goalToDeleteNameInput) || !_goalManager.ValidateGoalExists(goalToDeleteNameInput))
            {
                AnsiConsole.MarkupLine("[red][italic]\nNo coding goal with this name was found. Try again or type 0 to return to Main Menu: \n[/][/]");
                goalToDeleteNameInput = Console.ReadLine();
                if (goalToDeleteNameInput == "0")
                {
                    MainMenu();
                }
            }

            _goalManager.DeleteGoal(goalToDeleteNameInput);
            AnsiConsole.MarkupLine($"[green bold]\nSuccessfully deleted goal '{goalToDeleteNameInput}'![/]");
        }

        private void SetNewGoal()
        {
            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease select goal period type: (days/weeks/years). Type 0 to return to main menu.\n[/][/]");
            string? goalTypeInput = Console.ReadLine();
            string goalType = Validation.ValidateFilter(goalTypeInput);
            if (goalType == "0")
            {
                MainMenu();
            }

            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter goal name (must contain 1 or more letter). Type 0 to return to main menu.\n[/][/]");
            string? goalNameInput = Console.ReadLine();
            string goalName = Validation.ValidateGoalName(goalNameInput);
            if (goalName == "0")
            {
                MainMenu();
            }

            while (_goalManager.ValidateGoalExists(goalNameInput))
            {
                AnsiConsole.MarkupLine($"[red][italic]Goal with {goalNameInput} already exists. Try again or type 0 to return to Main Menu:\n[/][/]");
                goalNameInput = Console.ReadLine();
                goalName = Validation.ValidateGoalName(goalNameInput);
                if (goalName == "0")
                {
                    MainMenu();
                }
            }

            DateTime goalStart;
            DateTime endDate;
            string? codingTimeInput = "";
            string? codingTimeString = "";
            TimeSpan codingTimeTarget;

            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter coding time you are aiming to hit (Format: hh:mm). Type 0 to return to main menu.\n[/][/]");
            codingTimeInput = Console.ReadLine();
            codingTimeString = Validation.ValidateTime(codingTimeInput);

            switch (goalType)
            {
                case "days":
                    AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter last date to reach your goal: (Format: dd-MM-yyyy). Type 0 to return to main menu.\n[/][/]");
                    string? endDateInput  = Console.ReadLine();
                    string endDateString = Validation.ValidateDate(endDateInput);
                    if (endDateString == "0")
                    {
                        MainMenu();
                    }

                    goalStart = DateTime.Now.Date;
                    endDate = DateTime.ParseExact(endDateString, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                    while (goalStart > endDate)
                    {
                        AnsiConsole.MarkupLine("[red][italic]\nEnd date cannot be a past date. Try again (Format: dd-MM-yyyy) or type 0 to return to Main Menu: \n[/][/]");

                        endDateInput = Console.ReadLine();
                        endDateString = Validation.ValidateDate(endDateInput);
                        if (endDateString == "0")
                        {
                            MainMenu();
                        }
                        endDate = DateTime.ParseExact(endDateString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    }
                    AnsiConsole.MarkupLine($"[green bold]\nSuccesfully set goal for {(endDate - goalStart).Days} day(s) from now![/]");
                    break;

                case "weeks":
                    AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter number of weeks to reach your goal. Type 0 to return to main menu.\n[/][/]");
                    string? weeksInput = Console.ReadLine();
                    if (weeksInput == "0")
                    {
                        MainMenu();
                    }

                    while (!int.TryParse(weeksInput, out _) || int.Parse(weeksInput) < 1)
                    {
                        AnsiConsole.MarkupLine($"[red][italic]\nInvalid number of weeks. [yellow bold]Please enter an integer greater than 0: \n[/][/][/]");
                        weeksInput = Console.ReadLine();
                    }

                    if (weeksInput == "0")
                    {
                        MainMenu();
                    }

                    int numberOfWeeks = int.Parse(weeksInput);

                    goalStart = DateTime.Now;
                    endDate = goalStart.AddDays(numberOfWeeks * 7);
                    AnsiConsole.MarkupLine($"[green bold]\nSuccesfully set goal for {numberOfWeeks} week(s) from now![/]");
                    break;

                case "years":
                    AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter number of years to reach your goal. Type 0 to return to main menu.\n[/][/]");
                    string? yearsInput = Console.ReadLine();
                    if (yearsInput == "0")
                    {
                        MainMenu();
                    }

                    while (!int.TryParse(yearsInput, out _) || int.Parse(yearsInput) < 1)
                    {
                        AnsiConsole.MarkupLine($"[red][italic]\nInvalid number of years. [yellow bold]Please enter an integer greater than 0: \n[/][/][/]");
                        weeksInput = Console.ReadLine();
                    }

                    if (yearsInput == "0")
                    {
                        MainMenu();
                    }

                    int numberOfYears = int.Parse(yearsInput);

                    goalStart = DateTime.Now;
                    endDate = goalStart.AddYears(numberOfYears);
                    AnsiConsole.MarkupLine($"[green bold]\nSuccesfully set goal for {numberOfYears} year(s) from now![/]");
                    break;

                default:
                    endDate = goalStart = DateTime.Now; // won't happen
                    break;

            }

            codingTimeTarget = TimeSpan.ParseExact(codingTimeString, "hh\\:mm", CultureInfo.CurrentCulture);
            _goalManager.AddGoal(new CodingGoal(goalName, goalStart, codingTimeTarget, endDate));
        }

        private void ShowReport()
        {
            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease select period type: (days/weeks/years). Type 0 to return to main menu.\n[/][/]");
            string? filterInput = Console.ReadLine();
            string filterType = Validation.ValidateFilter(filterInput);

            List<CodingSession> allRecords = _db.GetAllRecords();

            TimeSpan totalCodingTime, averageCodingTime;
            List<CodingSession> filteredRecords;
            switch (filterType)
            {
                case "days":
                    AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease select starting date: (Format: dd-MM-yyyy): \n[/][/]");
                    string? startDateInput = Console.ReadLine();
                    DateTime startDate = DateTime.ParseExact(Validation.ValidateDate(startDateInput), "dd-MM-yyyy", CultureInfo.InvariantCulture);

                    AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease select end" +
                        $" date: (Format: dd-MM-yyyy) or type 1 to only filter by previous date: \n[/][/]");
                    string? endDateInput = Console.ReadLine();
                    DateTime endDate = (endDateInput == "1") ? startDate
                        : DateTime.ParseExact(Validation.ValidateDate(endDateInput), "dd-MM-yyyy", CultureInfo.InvariantCulture);

                    while (startDate > endDate)
                    {
                        AnsiConsole.MarkupLine($"[red][italic]\nEnd date cannot be less than starting date. [yellow bold]Enter valid end date (Format: dd-MM-yyyy) or type 0 to return to Main Menu: \n[/][/][/]");
                        endDateInput = Console.ReadLine();

                        endDate = DateTime.ParseExact(Validation.ValidateDate(endDateInput), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        if (endDateInput == "0")
                        {
                            MainMenu();
                        }
                    }

                    filteredRecords = allRecords.Where(r =>
                    {
                        DateTime recordDate = DateTime.ParseExact(r.Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        return recordDate >= startDate && recordDate <= endDate;
                    }).ToList();

                    break;

                case "weeks":
                    AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease select the starting date: (Format: dd-MM-yyyy): \n[/][/]");
                    string? startOfWeekInput = Console.ReadLine();
                    DateTime startOfWeek = DateTime.ParseExact(Validation.ValidateDate(startOfWeekInput), "dd-MM-yyyy", CultureInfo.InvariantCulture);

                    AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease select number of weeks (1-...) : \n[/][/]");
                    string? weeksInput = Console.ReadLine();
                    while (!int.TryParse(weeksInput, out _) || int.Parse(weeksInput) < 1)
                    {
                        AnsiConsole.MarkupLine($"[red][italic]\nInvalid weeks. [yellow bold]Please enter an integer greater than 0 : \n[/][/][/]");
                        weeksInput = Console.ReadLine();
                    }
                    DateTime endDateWeeks = startOfWeek.AddDays(double.Parse(weeksInput) * 7);

                    filteredRecords = allRecords.Where(r =>
                    {
                        DateTime recordDate = DateTime.ParseExact(r.Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        return recordDate >= startOfWeek && recordDate <= endDateWeeks;
                    }).ToList();
  
                    break;

                case "years":
                    AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease select starting year or type 0 to return to Main Menu: \n[/][/]");
                    string? startYearInput = Console.ReadLine();

                    int startYear = Validation.ValidateYear(startYearInput);
                    if (startYear == 0)
                    {
                        MainMenu();
                    }

                    AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease select end year : \n[/][/]");
                    string? endYearInput = Console.ReadLine();

                    int endYear = Validation.ValidateYear(endYearInput);
                    if (endYear == 0)
                    {
                        MainMenu();
                    }

                    while (startYear > endYear)
                    {
                        AnsiConsole.MarkupLine($"[red][italic]\nEnd year cannot be less than starting year. [yellow bold]Enter valid end year or type 0 to return to Main Menu: \n[/][/][/]");
                        endYearInput = Console.ReadLine();
                        endYear = Validation.ValidateYear(endYearInput);

                        if (endYear == 0)
                        {
                            MainMenu();
                        }
                    }

                    filteredRecords = allRecords.Where(r =>
                    {
                        DateTime recordDate = DateTime.ParseExact(r.Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        return recordDate.Year >= startYear && recordDate.Year <= endYear;
                    }).ToList();
 
                    break;
                default:
                    filteredRecords = Enumerable.Empty<CodingSession>().ToList();
                    AnsiConsole.MarkupLine("[green bold]\nNo matching records found.\n[/]");
                    return;
            }

            totalCodingTime = filteredRecords.Select(r => TimeSpan.ParseExact(r.Duration, "hh\\:mm", CultureInfo.InvariantCulture))
                        .Aggregate((accumulator, duration) => accumulator + duration);

            averageCodingTime = TimeSpan.FromTicks(totalCodingTime.Ticks / filteredRecords.Count);

            AnsiConsole.MarkupLine($"[green bold]\nTotal coding time: {totalCodingTime.ToString("hh\\:mm")} hours.[/]");
            AnsiConsole.MarkupLine($"[green bold]Average coding time: {(int)averageCodingTime.TotalHours:D2}:{averageCodingTime.Minutes:D2} hours.\n[/]");
        }

        private void Filter()
        {
            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter order in which to display records by date (ascending/descending) or type 0 to return to main menu: \n[/][/]");
            string? orderInput = Console.ReadLine();
            string orderType = Validation.ValidateOrder(orderInput);

            List<CodingSession> allRecords = _db.GetAllRecordsByOrder(orderType);

            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease select filter type: (days/weeks/years). Type 0 to return to main menu.\n[/][/]");
            string? filterInput = Console.ReadLine();
            string filterType = Validation.ValidateFilter(filterInput);

            List<CodingSession> filteredRecords;
            switch (filterType)
            {
                case "days":
                    AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease select starting date: (Format: dd-MM-yyyy): \n[/][/]");
                    string? startDateInput = Console.ReadLine();
                    DateTime startDate = DateTime.ParseExact(Validation.ValidateDate(startDateInput), "dd-MM-yyyy", CultureInfo.InvariantCulture);

                    AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease select end" +
                        $" date: (Format: dd-MM-yyyy) or type 1 to only filter by previous date: \n[/][/]");
                    string? endDateInput = Console.ReadLine();
                    DateTime endDate = (endDateInput == "1") ? startDate 
                        : DateTime.ParseExact(Validation.ValidateDate(endDateInput), "dd-MM-yyyy", CultureInfo.InvariantCulture);

                    while (startDate > endDate)
                    {
                        AnsiConsole.MarkupLine($"[red][italic]\nEnd date cannot be less than starting date. [yellow bold]Enter valid end date (Format: dd-MM-yyyy) or type 0 to return to Main Menu: \n[/][/][/]");
                        endDateInput = Console.ReadLine();
                        
                        endDate = DateTime.ParseExact(Validation.ValidateDate(endDateInput), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        if (endDateInput == "0")
                        {
                            MainMenu();
                        }
                    }

                    filteredRecords = allRecords.Where(r =>
                    {
                        DateTime recordDate = DateTime.ParseExact(r.Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        return recordDate >= startDate && recordDate <= endDate;
                    }).ToList();

                    DisplayTable(filteredRecords.ToArray());
                    break;

                case "weeks":
                    AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease select the starting date: (Format: dd-MM-yyyy): \n[/][/]");
                    string? startOfWeekInput = Console.ReadLine();
                    DateTime startOfWeek = DateTime.ParseExact(Validation.ValidateDate(startOfWeekInput), "dd-MM-yyyy", CultureInfo.InvariantCulture);

                    AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease select number of weeks (1-...) : \n[/][/]");
                    string? weeksInput = Console.ReadLine();
                    while (!int.TryParse(weeksInput, out _) || int.Parse(weeksInput) < 1)
                    {
                        AnsiConsole.MarkupLine($"[red][italic]\nInvalid weeks. [yellow bold]Please enter an integer greater than 0 : \n[/][/][/]");
                        weeksInput = Console.ReadLine();
                    }
                    DateTime endDateWeeks = startOfWeek.AddDays(double.Parse(weeksInput) * 7);

                    filteredRecords = allRecords.Where(r =>
                    {
                        DateTime recordDate = DateTime.ParseExact(r.Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        return recordDate >= startOfWeek && recordDate <= endDateWeeks;
                    }).ToList();

                    DisplayTable(filteredRecords.ToArray());
                    break;

                case "years":
                    AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease select starting year or type 0 to return to Main Menu: \n[/][/]");
                    string? startYearInput = Console.ReadLine();
                    
                    int startYear = Validation.ValidateYear(startYearInput);
                    if (startYear == 0)
                    {
                        MainMenu();
                    }

                    AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease select end year : \n[/][/]");
                    string? endYearInput = Console.ReadLine();

                    int endYear = Validation.ValidateYear(endYearInput);
                    if (endYear == 0)
                    {
                        MainMenu();
                    }

                    while (startYear > endYear)
                    {
                        AnsiConsole.MarkupLine($"[red][italic]\nEnd year cannot be less than starting year. [yellow bold]Enter valid end year or type 0 to return to Main Menu: \n[/][/][/]");
                        endYearInput = Console.ReadLine();
                        endYear = Validation.ValidateYear(endYearInput);

                        if (endYear == 0)
                        {
                            MainMenu();
                        }
                    }

                    filteredRecords = allRecords.Where(r =>
                    {
                        DateTime recordDate = DateTime.ParseExact(r.Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        return recordDate.Year >= startYear && recordDate.Year <= endYear;
                    }).ToList();

                    DisplayTable(filteredRecords.ToArray());
                    break;

                default:
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
                Validation.DisplayInvalidId(id);

                idInput = Console.ReadLine();

                id = Validation.ValidateId(idInput);

                if (id == 0)
                {
                    MainMenu();
                    break;
                }

                codingSession = _db.GetRecordById(id);
            }
            DisplayTable(codingSession);
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
                Validation.DisplayInvalidId(id); ;

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
            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter order in which to display records by date (ascending/descending) or type 0 to return to main menu: \n[/][/]");
            string? orderInput = Console.ReadLine();
            string orderType = Validation.ValidateOrder(orderInput);

            List<CodingSession> allRecords = _db.GetAllRecordsByOrder(orderType);

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
                Validation.DisplayInvalidId(id);

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

        private void DisplayTable(params CodingSession[] records)
        {
            if (!records.Any())
            {
                AnsiConsole.MarkupLine("[green bold]\nNo matching records found.\n[/]");
                return;
            }
            Table table = new Table();
            foreach (var property in typeof(CodingSession).GetProperties())
            {
                table.AddColumn(property.Name.ToString(), c => c.Centered());
            }
            foreach (var codingSession in records)
            {
                table.AddRow(new string[] { codingSession.Id.ToString(), codingSession.Date, codingSession.StartTime, codingSession.EndTime, codingSession.Duration });
            }
            table.Border(TableBorder.Rounded);
            table.ShowRowSeparators();
            AnsiConsole.Write(table);
        }
    }
}
