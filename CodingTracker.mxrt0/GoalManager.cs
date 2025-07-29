using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Spectre.Console;

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

        public bool ValidateGoalExists(string? goalName = "")
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
                Table table = new Table();
                foreach (var property in typeof(CodingGoal).GetProperties())
                {
                    table.AddColumn(property.Name.ToString(), c => c.Centered());
                }

                var goalStartDate = goalToDisplay.StartDate.ToString("dd-MM-yyyy");
                var goalTimeTarget = goalToDisplay.TimeTarget.ToString("hh\\:mm");
                var goalDeadline = goalToDisplay.Deadline.ToString("dd-MM-yyyy");
                var goalCompletedTime = goalToDisplay.CompletedTime.ToString("hh\\:mm");

                table.AddRow(new string[] { goalToDisplay.Name, goalStartDate, goalCompletedTime, goalTimeTarget, goalDeadline, goalToDisplay.IsCompleted.ToString()});
                table.Border(TableBorder.Rounded);
                table.ShowRowSeparators();
                AnsiConsole.Write(table);
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
    }
}
