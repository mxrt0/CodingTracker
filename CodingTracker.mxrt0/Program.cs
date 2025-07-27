using System.Configuration;
using System.Collections.Specialized;
using Spectre.Console;
using Dapper;
using SQLitePCL;


namespace CodingTracker.mxrt0
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Batteries.Init();
            var db = new MyCodingTrackerDatabase();
            db.CreateDbTable();
            UserInput input = new UserInput(db);
            input.MainMenu();
        }
    }
}
