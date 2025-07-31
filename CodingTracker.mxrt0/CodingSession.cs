using CodingTracker.mxrt0.Contracts;

namespace CodingTracker.mxrt0
{
    public class CodingSession : IDisplayable
    {
        public CodingSession()
        {

        }
        public CodingSession(string date, string startTime, string endTime, string duration)
        {
            Date = date;
            StartTime = startTime;
            EndTime = endTime;
            Duration = duration;
        }

        public int Id { get; set; }
        public string Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Duration { get; set; }

    }
}
