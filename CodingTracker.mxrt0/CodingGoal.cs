using CodingTracker.mxrt0.Contracts;

namespace CodingTracker.mxrt0
{
    public class CodingGoal : IDisplayable
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
