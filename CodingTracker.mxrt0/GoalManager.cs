using Newtonsoft.Json;
using Spectre.Console;
using System.Globalization;

namespace CodingTracker.mxrt0
{
    public class GoalManager
    {
        private List<CodingGoal> _goals;
        private readonly string _goalsFilePath = @"..\..\..\goals.json";
        public GoalManager()
        {
            LoadGoals();
        }

        public void AddGoal(CodingGoal goal)
        {
            _goals.Add(goal);
            SaveGoals();
        }
        public void DeleteGoal(string goalName)
        {
            _goals.RemoveAll(g => g.Name == goalName);//_goals = _goals.Where(g => g.Name != goalName).ToList(); // should be only 1 goal
            SaveGoals();
        }

        public CodingGoal UpdateGoal(string goalName, string contributionTimeString)
        {
            var goalToUpdate = _goals.Find(g => g.Name == goalName);

            if (goalToUpdate is not null)
            {
                var contributionTime = TimeSpan.ParseExact(contributionTimeString, "hh\\:mm", CultureInfo.InvariantCulture);
                goalToUpdate.CompletedTime += contributionTime;

                if (CheckGoalReached(goalToUpdate))
                {
                    goalToUpdate.IsCompleted = true;
                }
            }

            SaveGoals();
            return goalToUpdate;
        }

        public string ValidateGoalWithNameExists(string? goalName = "")
        {
            while (string.IsNullOrEmpty(goalName.Trim()) || _goals.Any(g => string.Equals(g.Name, goalName, StringComparison.OrdinalIgnoreCase)))
            {
                AnsiConsole.MarkupLine("[red][italic]\nNo coding goal with this name was found. Try again or type 0 to return to Main Menu: \n[/][/]");
                goalName = Console.ReadLine();
                if (goalName == "0")
                {
                    return goalName;
                }
            }
            return goalName.Trim();
        }

        public bool CheckGoalExists(string? goalName = "")
        {
            return _goals?.Any(g => string.Equals(g.Name, goalName, StringComparison.OrdinalIgnoreCase)) ?? false;
        }
        public bool CheckGoalReached(CodingGoal goal)
        {
            return goal?.CompletedTime >= goal?.TimeTarget;
        }
        public void DisplayGoalProgress(string? goalName = "")
        {
            var goalToDisplay = _goals.Find(g => g.Name == goalName);

            if (goalToDisplay is not null)
            {
                AnsiConsole.WriteLine();
                Table table = new Table();
                foreach (var property in typeof(CodingGoal).GetProperties())
                {
                    if (property.Name != nameof(CodingGoal.IsCompleted))
                    {
                        table.AddColumn(property.Name.ToString(), c => c.Centered());
                    }
                }

                var goalStartDate = goalToDisplay.StartDate.ToString("dd-MM-yyyy");
                var goalTimeTarget = goalToDisplay.TimeTarget.ToString("hh\\:mm");
                var goalDeadline = goalToDisplay.Deadline.ToString("dd-MM-yyyy");
                var goalCompletedTime = goalToDisplay.CompletedTime.ToString("hh\\:mm");

                table.AddRow(new string[] { goalToDisplay.Name, goalStartDate, goalCompletedTime, goalTimeTarget, goalDeadline });
                table.Border(TableBorder.Rounded);
                table.ShowRowSeparators();
                AnsiConsole.Write(table);

                var remainingTime = goalToDisplay.TimeTarget - goalToDisplay.CompletedTime;
                AnsiConsole.MarkupLine($"[green bold]\nKeep coding, you need {remainingTime.ToString("hh\\:mm")} more hour(s)/minute(s) to reach this goal![/]");

                var dailyCodingToReachGoal = CalculateDailyCodingTimeToReachGoal(goalToDisplay.CompletedTime, goalToDisplay.TimeTarget, goalToDisplay.Deadline);
                AnsiConsole.MarkupLine($"[green bold]\nAverage daily coding time to reach goal in time: {dailyCodingToReachGoal.ToString("hh\\:mm")} hour(s)/minute(s).[/]");
            }
        }
        private void LoadGoals()
        {
            if (!File.Exists(_goalsFilePath))
            {
                _goals = new List<CodingGoal>();
            }
            else
            {
                var goalsString = File.ReadAllText(_goalsFilePath);
                _goals = JsonConvert.DeserializeObject<List<CodingGoal>>(goalsString) ?? new List<CodingGoal>();
            }
        }

        private void SaveGoals()
        {
            var serializedGoals = JsonConvert.SerializeObject(_goals, Formatting.Indented);
            File.WriteAllText(_goalsFilePath, serializedGoals);
        }

        public void DeleteAllGoals()
        {
            if (File.Exists(_goalsFilePath))
            {
                File.Delete(_goalsFilePath);
                AnsiConsole.MarkupLine("[green bold]\nSuccessfully deleted all goals![/]");
                LoadGoals();
                return;
            }

            AnsiConsole.MarkupLine("[red italic]\nYou have not set any goals yet![/]");
        }

        public TimeSpan CalculateDailyCodingTimeToReachGoal(TimeSpan completedTime, TimeSpan timeTarget, DateTime goalEndDate)
        {
            var remainingCodingTime = timeTarget - completedTime;

            int daysUntilEndDate = (goalEndDate - DateTime.Now.Date).Days;

            return remainingCodingTime.Divide(daysUntilEndDate);
        }

        public bool CheckNotEmpty()
        {
            if (!_goals.Any())
            {
                AnsiConsole.MarkupLine("[red][italic]No goals have been added.[/][/]");
                return false;
            }
            return true;
        }
    }
}
