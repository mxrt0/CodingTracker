using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Spectre.Console;

namespace CodingTracker.mxrt0
{
    public class CodingSession
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
        public string StartTime { get;set; }
        public string EndTime { get; set; }
        public string Duration { get; set; }

    }
}
