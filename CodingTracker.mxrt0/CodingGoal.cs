using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.mxrt0
{
    public class CodingGoal
    {
        public CodingGoal(string name, DateTime startDate, TimeSpan timeTarget, DateTime deadline)
        {
            Name = name;
            StartDate = startDate;
            TimeTarget = timeTarget;
            Deadline = deadline;
            IsCompleted = false;
            CompletedTime = TimeSpan.Zero;
        }

        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public TimeSpan CompletedTime { get; set; }
        public TimeSpan TimeTarget { get; set; }
        public DateTime Deadline { get; set; }
        public bool IsCompleted { get; set; }

    }
}
