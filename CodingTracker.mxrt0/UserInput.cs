using CodingTracker.mxrt0.Contracts;
using Spectre.Console;
using System.Globalization;


namespace CodingTracker.mxrt0
{
    public class UserInput
    {
        private readonly MyCodingTrackerDatabase _db;
        private readonly string InvalidCommandMessage = "[italic][red]\nInvalid command. [yellow bold]Please enter a [italic]number [italic][red]from 0-14!\n[/][/][/][/][/][/]";
        private readonly string EnterIdMessage = "[magenta2][slowblink]\nPlease enter the ID of the record you wish to view/update. Type 0 to return to main menu.\n[/][/]";
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
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 7 to [italic][yellow bold]View Record Report By Period[/][/][/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 8 to [italic][red bold]Delete All Records[/][/][/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 9 to [italic][chartreuse2 bold]Set New Coding Goal[/][/][/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 10 to [italic][red bold]Delete Coding Goal[/][/][/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 11 to [italic][magenta bold]Check Coding Goal Progress[/][/][/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 12 to [italic][yellow bold]Update Coding Goal Progress[/][/][/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 13 to [italic][orange3 bold]View All Coding Goals[/][/][/]");
                AnsiConsole.MarkupLine($"[mediumturquoise]Type 14 to [italic][red bold]Delete All Coding Goals\n[/][/][/]");

                string? userInput = Console.ReadLine();

                while (string.IsNullOrWhiteSpace(userInput) || !int.TryParse(userInput.Trim(), out _)
                    || int.Parse(userInput.Trim()) < 0 || int.Parse(userInput.Trim()) > 14)
                {
                    AnsiConsole.MarkupLine(InvalidCommandMessage);
                    userInput = Console.ReadLine();
                }

                AnsiConsole.Clear();
                HandleUserInput(userInput.Trim());
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
                    ViewAllRecords();
                    break;
                case "2":
                    FilterRecords();
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
                    DeleteAllRecords();
                    break;
                case "9":
                    SetNewGoal();
                    break;
                case "10":
                    DeleteGoal();
                    break;
                case "11":
                    CheckGoalProgress();
                    break;
                case "12":
                    UpdateGoalProgress();
                    break;
                case "13":
                    ViewAllGoals();
                    break;
                case "14":
                    DeleteAllGoals();
                    break;
                default:
                    break;
            }
        }

        private void ViewAllGoals()
        {
            if (!_goalManager.CheckNotEmpty())
            {
                return;
            }
            var allGoals = _goalManager.GetAllGoals();
            DisplayTable(allGoals.ToArray());
        }

        private void DeleteAllRecords()
        {
            if (!_db.CheckNotEmpty())
            {
                return;
            }

            ConfirmationPrompt confirmation = new("[magenta2][slowblink]\nAre you sure you want to permanently delete all your coding goals?\n[/][/]")
            {
                DefaultValue = false,
                InvalidChoiceMessage = "[red][italic]\nPlease enter 'Y' to [underline]Delete All Records[/] or 'N' to return to Main Menu: \n[/][/]"
            };
            var deleteAll = confirmation.Show(AnsiConsole.Console);

            if (deleteAll)
            {
                _db.DeleteAllRecords();
                AnsiConsole.MarkupLine("[green bold]\nSuccessfully deleted all records![/]");
            }
        }

        private void DeleteAllGoals()
        {
            if (!_goalManager.CheckNotEmpty())
            {
                return;
            }

            ConfirmationPrompt confirmation = new("[magenta2][slowblink]\nAre you sure you want to permanently delete all your coding goals?\n[/][/]")
            {
                DefaultValue = false,
                InvalidChoiceMessage = "[red][italic]\nPlease enter 'Y' to [underline]Delete All Records[/] or 'N' to return to Main Menu: \n[/][/]"
            };
            var deleteAll = confirmation.Show(AnsiConsole.Console);

            if (deleteAll)
            {
                _goalManager.DeleteAllGoals();
                AnsiConsole.MarkupLine("[green bold]\nSuccessfully deleted all goals![/]");
            }
        }

        private void UpdateGoalProgress()
        {
            if (!_goalManager.CheckNotEmpty())
            {
                return;
            }

            AnsiConsole.MarkupLine("[magenta2][slowblink]\nPlease enter the exact name of your goal. Type 0 to return to main menu.\n[/][/]");
            string? goalToUpdateNameInput = Console.ReadLine();
            CheckReturnToMainMenu(goalToUpdateNameInput);

            var goalToUpdateName = _goalManager.EnsureGoalNameValid(goalToUpdateNameInput);
            CheckReturnToMainMenu(goalToUpdateName);

            AnsiConsole.MarkupLine("[magenta2][slowblink]\nEnter the hours/minutes of coding you have done toward your goal (Format: hh:mm).\n[/][/]");
            string codingContributionTimeInput = Console.ReadLine();
            string codingContributionTime = Validation.ValidateTime(codingContributionTimeInput);
            CheckReturnToMainMenu(codingContributionTime);

            var updatedGoal = _goalManager.UpdateGoal(goalToUpdateName, codingContributionTime);

            if (updatedGoal.IsCompleted)
            {
                AnsiConsole.MarkupLine($"[green bold]Congratulations, you have reached goal '{updatedGoal.Name}'![/]");
                _goalManager.DeleteGoal(updatedGoal.Name);
            }
            else
            {
                _goalManager.DisplayGoalProgress(updatedGoal.Name);
            }
        }

        private void CheckGoalProgress()
        {
            if (!_goalManager.CheckNotEmpty())
            {
                return;
            }

            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter the exact name of your goal. Type 0 to return to main menu.\n[/][/]");
            string? goalToCheckNameInput = Console.ReadLine();
            CheckReturnToMainMenu(goalToCheckNameInput);

            var goalToCheckName = _goalManager.EnsureGoalNameValid(goalToCheckNameInput);
            CheckReturnToMainMenu(goalToCheckName);

            _goalManager.DisplayGoalProgress(goalToCheckName);
        }

        private void DeleteGoal()
        {
            if (!_goalManager.CheckNotEmpty())
            {
                return;
            }

            AnsiConsole.MarkupLine("[magenta2][slowblink]\nPlease enter the exact name of your goal. Type 0 to return to main menu.\n[/][/]");
            string? goalToDeleteNameInput = Console.ReadLine();
            CheckReturnToMainMenu(goalToDeleteNameInput);

            var goalToDeleteName = _goalManager.EnsureGoalNameValid(goalToDeleteNameInput);
            CheckReturnToMainMenu(goalToDeleteName);

            _goalManager.DeleteGoal(goalToDeleteName);
            AnsiConsole.MarkupLine($"[green bold]\nSuccessfully deleted goal '{goalToDeleteName}'![/]");
        }

        private void SetNewGoal()
        {
            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease select goal period type: (days/weeks/years). Type 0 to return to main menu.\n[/][/]");
            string? goalTypeInput = Console.ReadLine();
            CheckReturnToMainMenu(goalTypeInput);

            string goalType = Validation.ValidateFilter(goalTypeInput);
            CheckReturnToMainMenu(goalType);

            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter goal name (must contain 1 or more letter): \n[/][/]");
            string? goalNameInput = Console.ReadLine();

            string goalName = Validation.EnsureValidNewGoalName(goalNameInput);
            CheckReturnToMainMenu(goalName);

            while (_goalManager.CheckGoalExists(goalNameInput))
            {
                AnsiConsole.MarkupLine($"[red][italic]Goal with {goalNameInput} already exists. Try again or type 0 to return to Main Menu:\n[/][/]");
                goalNameInput = Console.ReadLine();
                CheckReturnToMainMenu(goalNameInput);

                goalName = Validation.EnsureValidNewGoalName(goalNameInput);
                CheckReturnToMainMenu(goalName);
            }

            DateTime goalStart;
            DateTime endDate;
            string? codingTimeInput = "";
            string? codingTimeString = "";
            TimeSpan codingTimeTarget;

            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter coding time you are aiming to hit (Format: hh:mm). Type 0 to return to main menu.\n[/][/]");
            codingTimeInput = Console.ReadLine();
            CheckReturnToMainMenu(codingTimeInput);

            codingTimeString = Validation.ValidateTime(codingTimeInput);
            CheckReturnToMainMenu(codingTimeString);

            switch (goalType)
            {
                case "days":
                    AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter last date to reach your goal: (Format: dd-MM-yyyy). Type 0 to return to main menu.\n[/][/]");
                    string? endDateInput = Console.ReadLine();
                    CheckReturnToMainMenu(endDateInput);

                    string endDateString = Validation.ValidateDate(endDateInput);
                    CheckReturnToMainMenu(endDateString);

                    goalStart = DateTime.Now.Date;
                    endDate = DateTime.ParseExact(endDateString, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                    while (goalStart > endDate)
                    {
                        AnsiConsole.MarkupLine("[red][italic]\nEnd date cannot be a past date. Try again (Format: dd-MM-yyyy) or type 0 to return to Main Menu: \n[/][/]");

                        endDateInput = Console.ReadLine();
                        CheckReturnToMainMenu(endDateInput);

                        endDateString = Validation.ValidateDate(endDateInput);
                        CheckReturnToMainMenu(endDateString);

                        endDate = DateTime.ParseExact(endDateString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    }
                    AnsiConsole.MarkupLine($"[green bold]\nSuccesfully set goal for {(endDate - goalStart).Days} day(s) from now![/]");
                    break;

                case "weeks":
                    AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter number of weeks to reach your goal. Type 0 to return to main menu.\n[/][/]");
                    string? weeksInput = Console.ReadLine();
                    CheckReturnToMainMenu(weeksInput);

                    int numberOfWeeks = Validation.ValidateWeeks(weeksInput);
                    CheckReturnToMainMenu(numberOfWeeks.ToString());

                    goalStart = DateTime.Now;
                    endDate = goalStart.AddDays(numberOfWeeks * 7);
                    AnsiConsole.MarkupLine($"[green bold]\nSuccesfully set goal for {numberOfWeeks} week(s) from now![/]");
                    break;

                case "years":
                    AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter number of years to reach your goal. Type 0 to return to main menu.\n[/][/]");
                    string? yearsInput = Console.ReadLine();
                    CheckReturnToMainMenu(yearsInput);

                    while (!int.TryParse(yearsInput, out _) || int.Parse(yearsInput) < 1)
                    {
                        AnsiConsole.MarkupLine($"[red][italic]\nInvalid number of years. [yellow bold]Please enter an integer greater than 0 or type 0 to return to Main Menu: \n[/][/][/]");
                        yearsInput = Console.ReadLine();
                        CheckReturnToMainMenu(yearsInput);
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
            if (!_db.CheckNotEmpty())
            {
                return;
            }

            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease select period type: (days/weeks/years). Type 0 to return to main menu.\n[/][/]");
            string? periodInput = Console.ReadLine();
            CheckReturnToMainMenu(periodInput);

            string periodType = Validation.ValidateFilter(periodInput);
            CheckReturnToMainMenu(periodType);

            List<CodingSession> allRecords = _db.GetAllRecords();

            TimeSpan totalCodingTime, averageCodingTime;

            var filteredRecords = FilterItems(allRecords, periodType);
            if (filteredRecords.Length == 0)
            {
                return;
            }

            totalCodingTime = filteredRecords.Select(r => TimeSpan.ParseExact(r.Duration, "hh\\:mm", CultureInfo.InvariantCulture))
                        .Aggregate((accumulator, duration) => accumulator + duration);

            averageCodingTime = TimeSpan.FromTicks(totalCodingTime.Ticks / filteredRecords.Length);

            AnsiConsole.MarkupLine($"[green bold]\nTotal coding time: {totalCodingTime.ToString("hh\\:mm")} hours.[/]");
            AnsiConsole.MarkupLine($"[green bold]Average coding time: {(int)averageCodingTime.TotalHours:D2}:{averageCodingTime.Minutes:D2} hours.\n[/]");
        }

        private void FilterRecords()
        {
            if (!_db.CheckNotEmpty())
            {
                return;
            }

            List<CodingSession> allRecords = _db.GetAllRecords();

            if (allRecords.Count > 1)
            {
                AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter order in which to display records by date (ascending/descending) or type 0 to return to main menu: \n[/][/]");
                string? orderInput = Console.ReadLine();
                CheckReturnToMainMenu(orderInput);

                string orderType = Validation.ValidateOrder(orderInput);
                CheckReturnToMainMenu(orderInput);

                allRecords = _db.GetAllRecordsByOrder(orderType);
            }


            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease select filter type: (days/weeks/years). Type 0 to return to main menu.\n[/][/]");
            string? filterInput = Console.ReadLine();
            CheckReturnToMainMenu(filterInput);

            string filterType = Validation.ValidateFilter(filterInput);
            CheckReturnToMainMenu(filterType);

            FilterItems(allRecords, filterType);
        }
        private void ViewRecord()
        {
            if (!_db.CheckNotEmpty())
            {
                return;
            }

            AnsiConsole.MarkupLine(EnterIdMessage);

            string? idInput = Console.ReadLine();

            int id = Validation.ValidateId(idInput);

            var codingSession = _db.GetRecordById(id);

            while (codingSession.Id == 0)
            {
                Validation.DisplayInvalidId(id);

                idInput = Console.ReadLine();

                id = Validation.ValidateId(idInput);

                CheckReturnToMainMenu(id.ToString());

                codingSession = _db.GetRecordById(id);
            }
            DisplayTable(codingSession);
        }

        private void UpdateRecord()
        {
            if (!_db.CheckNotEmpty())
            {
                return;
            }

            AnsiConsole.MarkupLine(EnterIdMessage);

            string? idInput = Console.ReadLine();
            CheckReturnToMainMenu(idInput);

            int id = Validation.ValidateId(idInput);
            CheckReturnToMainMenu(id.ToString());

            var codingSession = _db.GetRecordById(id);

            while (codingSession.Id == 0)
            {
                Validation.DisplayInvalidId(id);

                idInput = Console.ReadLine();
                CheckReturnToMainMenu(idInput);

                id = Validation.ValidateId(idInput);
                CheckReturnToMainMenu(id.ToString());

                codingSession = _db.GetRecordById(id);
            }

            var newDate = GetDateInput();

            var newStartTime = GetStartTimeInput();
            var newEndTime = GetEndTimeInput();

            var newDuration = CalculateDuration(ref newStartTime, ref newEndTime);

            _db.UpdateRecord(id, newDate, newStartTime, newEndTime, newDuration);

        }

        private void ViewAllRecords()
        {
            if (!_db.CheckNotEmpty())
            {
                return;
            }

            var allRecords = _db.GetAllRecords();

            if (allRecords.Count > 1)
            {
                AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter order in which to display records by date (ascending/descending) or type 0 to return to main menu: \n[/][/]");
                string? orderInput = Console.ReadLine();
                string orderType = Validation.ValidateOrder(orderInput);
                CheckReturnToMainMenu(orderType);
                allRecords = _db.GetAllRecordsByOrder(orderType);
            }

            DisplayTable(allRecords.ToArray());
        }

        private void DeleteRecord()
        {
            if (!_db.CheckNotEmpty())
            {
                return;
            }

            AnsiConsole.MarkupLine(EnterIdMessage);

            string? idInput = Console.ReadLine();

            int id = Validation.ValidateId(idInput);

            CheckReturnToMainMenu(id.ToString());

            var codingSession = _db.GetRecordById(id);

            while (codingSession.Id == 0)
            {
                Validation.DisplayInvalidId(id);

                idInput = Console.ReadLine();

                id = Validation.ValidateId(idInput);

                CheckReturnToMainMenu(id.ToString());

                codingSession = _db.GetRecordById(id);
            }

            _db.DeleteRecord(codingSession.Id);
        }

        private void AddRecord()
        {
            string date = GetDateInput();
            CheckReturnToMainMenu(date);

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
            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter the date: (Format: dd-MM-yyyy). Type 'Today' for current date or type 0 to return to Main Menu.\n[/][/]");
            string? userDateInput = Console.ReadLine();
            CheckReturnToMainMenu(userDateInput);

            if (userDateInput.Trim().ToLower() == "today")
            {
                return DateTime.Today.ToString("dd-MM-yyyy");
            }

            return Validation.ValidateDate(userDateInput);
        }

        private string GetStartTimeInput()
        {
            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter the start time (Format: hh:mm) or type 1 to track from current time. Type 0 to return to main menu.\n[/][/]");

            string? startTimeInput = Console.ReadLine();

            CheckReturnToMainMenu(startTimeInput);

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

            CheckReturnToMainMenu(input);

            var endTime = DateTime.Now.ToString("hh\\:mm");

            return (startTime, endTime);

        }

        private string GetEndTimeInput(bool tracking = false)
        {
            if (tracking)
            {
                var input = Console.ReadLine();

                CheckReturnToMainMenu(input);

                return DateTime.Now.ToString("hh\\:mm");
            }

            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease enter the end time: (Format: hh:mm). Type 0 to return to main menu.\n[/][/]");

            string? endTimeInput = Console.ReadLine();

            CheckReturnToMainMenu(endTimeInput);

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

        private void DisplayTable<T>(params T[] records) where T : IDisplayable
        {
            if (!records.Any())
            {
                AnsiConsole.MarkupLine("[green bold]\nNo matching records found.\n[/]");
                return;
            }
            Table table = new Table();
            foreach (var property in typeof(T).GetProperties())
            {
                if (property.Name != nameof(CodingGoal.IsCompleted))
                {
                    table.AddColumn(property.Name.ToString(), c => c.Centered());
                }

            }
            foreach (var item in records)
            {
                if (item is CodingSession codingSession)
                {
                    table.AddRow(new string[] { codingSession.Id.ToString(), codingSession.Date, codingSession.StartTime, codingSession.EndTime, codingSession.Duration });
                }
                else if (item is CodingGoal goal)
                {
                    table.AddRow(new string[] { goal.Name, goal.StartDate.ToString("dd-MM-yyyy"),
                        goal.CompletedTime.ToString("hh\\:mm"), goal.TimeTarget.ToString("hh\\:mm"), goal.Deadline.ToString("dd-MM-yyyy") });
                }

            }
            table.Border(TableBorder.Rounded);
            table.ShowRowSeparators();
            AnsiConsole.Write(table);
        }

        private void CheckReturnToMainMenu(string? input = "")
        {
            if (input == "0")
            {
                MainMenu();
            }
        }

        private (DateTime startDate, DateTime endDate) FilterByDays()
        {
            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease select starting date: (Format: dd-MM-yyyy): \n[/][/]");
            string? startDateInput = Console.ReadLine();
            string validatedStartDate = Validation.ValidateDate(startDateInput);
            CheckReturnToMainMenu(validatedStartDate);
            DateTime startDate = DateTime.ParseExact(validatedStartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease select end" +
                $" date: (Format: dd-MM-yyyy) or type 1 to only filter by previous date: \n[/][/]");
            string? endDateInput = Console.ReadLine();
            DateTime endDate;
            if (endDateInput == "1")
            {
                endDate = startDate;
            }
            else
            {
                string validatedEndDate = Validation.ValidateDate(endDateInput);
                CheckReturnToMainMenu(validatedEndDate);

                endDate = DateTime.ParseExact(validatedEndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                while (startDate > endDate)
                {
                    AnsiConsole.MarkupLine($"[red][italic]\nEnd date cannot be less than starting date. [yellow bold]Enter valid end date (Format: dd-MM-yyyy) or type 0 to return to Main Menu: \n[/][/][/]");
                    endDateInput = Console.ReadLine();
                    CheckReturnToMainMenu(endDateInput);

                    validatedEndDate = Validation.ValidateDate(endDateInput);
                    CheckReturnToMainMenu(validatedEndDate);

                    endDate = DateTime.ParseExact(validatedEndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                }
            }

            return (startDate, endDate);
        }

        private (DateTime startOfWeek, DateTime endDateWeeks) FilterByWeeks()
        {
            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease select the starting date: (Format: dd-MM-yyyy): \n[/][/]");
            string? startOfWeekInput = Console.ReadLine();
            CheckReturnToMainMenu(startOfWeekInput);

            string validatedStartOfWeek = Validation.ValidateDate(startOfWeekInput);
            CheckReturnToMainMenu(validatedStartOfWeek);
            DateTime startOfWeek = DateTime.ParseExact(validatedStartOfWeek, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease select number of weeks (1-...) : \n[/][/]");
            string? weeksInput = Console.ReadLine();

            while (!int.TryParse(weeksInput, out _) || int.Parse(weeksInput) < 1)
            {
                AnsiConsole.MarkupLine($"[red][italic]\nInvalid weeks. [yellow bold]Please enter an integer greater than 0 or type 0 to return to Main Menu: \n[/][/][/]");
                weeksInput = Console.ReadLine();
                CheckReturnToMainMenu(weeksInput);
            }
            DateTime endDateWeeks = startOfWeek.AddDays(double.Parse(weeksInput) * 7);

            return (startOfWeek, endDateWeeks);
        }

        private (int startYear, int endYear) FilterByYears()
        {
            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease select starting year or type 0 to return to Main Menu: \n[/][/]");
            string? startYearInput = Console.ReadLine();
            CheckReturnToMainMenu(startYearInput);

            int startYear = Validation.ValidateYears(startYearInput);
            CheckReturnToMainMenu(startYear.ToString());

            AnsiConsole.MarkupLine($"[magenta2][slowblink]\nPlease select end year : \n[/][/]");
            string? endYearInput = Console.ReadLine();

            int endYear = Validation.ValidateYears(endYearInput);
            CheckReturnToMainMenu(endYear.ToString());

            while (startYear > endYear)
            {
                AnsiConsole.MarkupLine($"[red][italic]\nEnd year cannot be less than starting year. [yellow bold]Enter valid end year or type 0 to return to Main Menu: \n[/][/][/]");
                endYearInput = Console.ReadLine();
                CheckReturnToMainMenu(endYearInput);

                endYear = Validation.ValidateYears(endYearInput);
                CheckReturnToMainMenu(endYear.ToString());
            }
            return (startYear, endYear);
        }
        private CodingSession[] FilterItems(List<CodingSession> allRecords, string filterType)
        {
            CodingSession[] filteredRecords;
            switch (filterType)
            {
                case "days":
                    (DateTime StartDate, DateTime EndDate) filterDaysResult = FilterByDays();

                    filteredRecords = allRecords.Where(r =>
                    {
                        DateTime recordDate = DateTime.ParseExact(r.Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        return recordDate >= filterDaysResult.StartDate && recordDate <= filterDaysResult.EndDate;
                    }).ToArray();

                    DisplayTable(filteredRecords);
                    break;

                case "weeks":
                    (DateTime StartOfWeek, DateTime EndDateWeeks) filterWeeksResult = FilterByWeeks();

                    filteredRecords = allRecords.Where(r =>
                    {
                        DateTime recordDate = DateTime.ParseExact(r.Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        return recordDate >= filterWeeksResult.StartOfWeek && recordDate <= filterWeeksResult.EndDateWeeks;
                    }).ToArray();

                    DisplayTable(filteredRecords);
                    break;

                case "years":
                    (int StartYear, int EndYear) filterYearsResult = FilterByYears();

                    filteredRecords = allRecords.Where(r =>
                    {
                        DateTime recordDate = DateTime.ParseExact(r.Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        return recordDate.Year >= filterYearsResult.StartYear && recordDate.Year <= filterYearsResult.EndYear;
                    }).ToArray();

                    DisplayTable(filteredRecords);
                    break;
                default:
                    AnsiConsole.MarkupLine("[green bold]\nNo matching records found.\n[/]");
                    return Array.Empty<CodingSession>();
            }
            return filteredRecords;
        }
    }
}
