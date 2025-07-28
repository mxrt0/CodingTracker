using System;
using System.Collections.Generic;
using System.Configuration;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Data.SqlClient;
using SQLitePCL;
using System.Net.Quic;
using Spectre.Console;

namespace CodingTracker.mxrt0
{
    public class MyCodingTrackerDatabase
    {
        private readonly string _connectionStringKey;
        private readonly string _dbPath;

        public MyCodingTrackerDatabase() : this("DefaultConnectionString", "DefaultDatabasePath")
        {

        }

        public MyCodingTrackerDatabase(string connectionStringKey, string dbPath)
        {
            _connectionStringKey = connectionStringKey;
            _dbPath = dbPath;
        }

        public string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings[_connectionStringKey].ConnectionString;
        }
        public string GetDbPath()
        {
            var path = ConfigurationManager.AppSettings[_dbPath];

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new InvalidOperationException("Database path is missing or empty.");
            }
            return path;         
        }
        public void CreateDbTable()
        {
            string dbPath = GetDbPath();
            if (!File.Exists(dbPath))
            {
                string connectionString = GetConnectionString();
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    var createTableCommand = $@"
                            CREATE TABLE IF NOT EXISTS codingTracker (
                                Id INTEGER PRIMARY KEY,
                                Date TEXT NOT NULL,
                                StartTime TEXT NOT NULL,
                                EndTime TEXT NOT NULL,
                                Duration TEXT
                            )";
                    connection.Execute(createTableCommand);
                }
            }
        }

        public List<CodingSession> GetAllRecords()
        {
            string connectionString = GetConnectionString();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var getAllCommand = @"
                            SELECT * FROM codingTracker";

                var allRecords = connection.Query<CodingSession>(getAllCommand);

                return allRecords.ToList();
            }
        }

        public void InsertNewRecord(CodingSession cs)
        {
            string connectionString = GetConnectionString();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var insertCommand = @"
                            INSERT INTO codingTracker (Date, StartTime, EndTime, Duration) VALUES (@Date, @StartTime, @EndTime, @Duration)";

                connection.Execute(insertCommand, new { Date = cs.Date, StartTime = cs.StartTime, EndTime = cs.EndTime, Duration = cs.Duration });

                AnsiConsole.MarkupLine("[green1 bold]\nSuccessfully inserted new record!\n[/]");
            }
            
        }

        public void DeleteRecord(int id)
        {
            string connectionString = GetConnectionString();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var deleteCommand = @"
                            DELETE FROM codingTracker WHERE Id = @Id";

                connection.Execute(deleteCommand, new { Id = id });

                AnsiConsole.MarkupLine($"[green1 bold]\nSuccessfully deleted record with ID {id}!\n[/]");
            }    
        }
        
        public CodingSession GetRecordById(int id)
        {
            string connectionString = GetConnectionString();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var getByIdCommand = @"
                            SELECT Id, Date, StartTime, EndTime, Duration FROM codingTracker WHERE Id = @Id";

                var codingSession = connection.QueryFirstOrDefault<CodingSession>(getByIdCommand, new { Id = id });

                codingSession ??= new CodingSession {Id = 0};

                return codingSession;
            }
        }

        public void UpdateRecord(int id, string newDate, string newStartTime, string newEndTime, string newDuration)
        {
            string connectionString = GetConnectionString();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var updateCommand = @"
                            UPDATE codingTracker SET Date = @Date, StartTime = @StartTime, EndTime = @EndTime, Duration = @Duration WHERE Id = @Id";

                connection.Execute(updateCommand, new { Date = newDate, StartTime = newStartTime, EndTime = newEndTime, Duration = newDuration, Id = id });

                AnsiConsole.MarkupLine($"[green1 bold]\nSuccessfully updated record with ID {id}!\n[/]");
            }
        }
    }
}
