using System.Configuration;
using System.Globalization;
using Microsoft.Data.Sqlite;
using Dapper;

namespace coding_tracker
{
    /// <summary>
    /// Responsible for handling all interactions with the database (DB)
    /// </summary>
    class DBController
    {
        static string connectionString = @ConfigurationManager.AppSettings.Get("ConnectionString");
        static string databasePath = @ConfigurationManager.AppSettings.Get("DatabasePath");
        static string completeConnectionString = connectionString + databasePath;

        static internal DateTime? SessionStartTime;  // The recorded session's start time
        static internal DateTime? SessionEndTime;  // The recorded session's end time

        static internal string GetSessionStartTime()
        {
            if (SessionStartTime != null)
            {
                return SessionStartTime.ToString();
            }
            else
            {
                Console.WriteLine("No session has begun.");
                return "";
            }
        }

        internal static void InitialiseDB()
        {
            using (var connection = new SqliteConnection(completeConnectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText =
                    @"CREATE TABLE IF NOT EXISTS hours_playefod (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Date DATE,
                        StartTime INTEGER,
                        EndTime INTEGER
                        )";

                tableCmd.ExecuteNonQuery();

                // Check if the table is empty, if so, seed it
                var checkCmd = connection.CreateCommand();
                checkCmd.CommandText = "SELECT COUNT(*) FROM hours_played";
                var count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count == 0)
                {
                    SeedDatabase(connection);
                }

                connection.Close();
            }
        }

        internal static void SeedDatabase(SqliteConnection connection)
        {
            Console.Clear();
            Console.WriteLine("Please wait, seeding database on first initialisation (up to 20 seconds...)");

            var random = new Random();
            var insertCmd = connection.CreateCommand();
            var numberOfRecords = 100;
            var maxMinutes = 59;
            var minStartTime = 6;   // 06:00
            var maxStartTime = 13;  // 13:00
            var minDuration = 20;  // Minutes
            var maxDuration = 150;  // Minutes

            for (int i = 0; i < numberOfRecords; i++)
            {
                string randomDate = GenerateRandomDate(random);
                DateTime parsedDate = DateTime.ParseExact(randomDate, "dd-MM-yy", CultureInfo.InvariantCulture);

                int randomStartHour = random.Next(minStartTime, maxStartTime);
                int randomStartMinute = random.Next(0, maxMinutes);
                DateTime startTime = new DateTime(1, 1, 1, randomStartHour, randomStartMinute, 0);
                int randomDurationMinutes = random.Next(minDuration, maxDuration);

                DateTime endTime = startTime.AddMinutes(randomDurationMinutes);

                int formattedStartTime = int.Parse(startTime.ToString("HHmm"));
                int formattedEndTime = int.Parse(endTime.ToString("HHmm"));

                // Dapper insertion
                string sql = "INSERT INTO hours_played (Date, StartTime, EndTime) VALUES (@Date, @StartTime, @EndTime)";
                connection.Execute(sql, new
                {
                    Date = parsedDate,
                    StartTime = formattedStartTime,
                    EndTime = formattedEndTime
                });
            }
        }

        internal static List<CodingSession> GetAllRecords()
        {
            Console.Clear();

            using (var connection = new SqliteConnection(completeConnectionString))
            {
                connection.Open();

                string sql = "SELECT * FROM hours_played";

                List<CodingSession> allRecords = connection.Query<CodingSession>(sql).ToList();

                connection.Close();

                return allRecords;
            }
        }

        internal static void Insert()
        {
            string? date = UserInput.GetDateInput();

            int? startTime = UserInput.GetFourDigitTimeInput("\n\nWhat time did you start the session? (24hr time, i.e '1300' for 1:00pm):\n");
            int? endTime = UserInput.GetFourDigitTimeInput("\n\nWhat time did you end the session? (24hr time):\n");

            using (var connection = new SqliteConnection(completeConnectionString))
            {
                connection.Open();

                string sql = "INSERT INTO hours_played (Date, StartTime, EndTime) VALUES (@Date, @StartTime, @EndTime)";

                // Dapper
                connection.Execute(sql, new
                {
                    Date = date,
                    StartTime = startTime,
                    EndTime = endTime
                });

                connection.Close();
            }
        }

        internal static void Delete()
        {
            Console.Clear();
            GetAllRecords();

            var recordId = UserInput.GetNumberInput("\n\nPlease type the Id of the record you want to delete or type 0 to go back to Main Menu\n\n");

            if (recordId == 0)
            {
                Console.Clear();
                return;
            }

            using (var connection = new SqliteConnection(completeConnectionString))
            {
                connection.Open();

                string sql = "DELETE FROM hours_played WHERE Id = @RecordId";

                // Use Dapper
                int rowCount = connection.Execute(sql, new { RecordId = recordId });

                if (rowCount == 0)
                {
                    Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist. Press any key to try again...\n\n");
                    Console.ReadKey();
                    connection.Close();
                    Delete();
                    return;
                }
            }

            Console.Clear();
            Console.WriteLine($"\n\nRecord with Id {recordId} was deleted. Press any key to continue...\n\n");
            Console.ReadKey();
        }

        internal static void Update()
        {
            GetAllRecords();

            var recordId = UserInput.GetNumberInput("\n\nPlease type Id of the record would like to update. Type 0 to return to main manu.\n\n");

            if (recordId == 0)
            {
                Console.Clear();
                return;
            }

            using (var connection = new SqliteConnection(completeConnectionString))
            {
                connection.Open();

                string checkQuery = "SELECT COUNT(1) FROM hours_played WHERE Id = @RecordId";
                int checkQueryResult = connection.ExecuteScalar<int>(checkQuery, new { RecordId = recordId });

                if (checkQueryResult == 0)
                {
                    Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist. Press any key to try again...\n\n");
                    Console.ReadKey();
                    connection.Close();
                    Update();
                    return;
                }

                string? date = UserInput.GetDateInput();
                int? startTime = UserInput.GetFourDigitTimeInput("\n\nWhat time did you start the session? (24hr time, i.e '1300' for 1:00pm):\n");
                int? endTime = UserInput.GetFourDigitTimeInput("\n\nWhat time did you end the session? (24hr time):\n");

                string updateQuery = "UPDATE hours_played SET Date = @Date, StartTime = @StartTime, EndTime = @EndTime WHERE Id = @RecordId";

                // Use Dapper
                int affectedRows = connection.Execute(updateQuery, new
                {
                    Date = date,
                    StartTime = startTime,
                    EndTime = endTime,
                    RecordId = recordId
                });

                Console.Clear();

                if (affectedRows > 0)
                {
                    Console.WriteLine("Record updated successfully, press any key to continue...");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Failed to update the record, press any key to try again...");
                    Console.ReadKey();
                    Update();
                    return;
                }

                connection.Close();
            }
        }

        /// <summary>
        /// Counts the number of times a session was performed in a given year (YY)
        /// </summary>
        internal static void GetReportForAYear()
        {
            int codingSessionCount = 0;

            Console.WriteLine($"\n\nWhich year would you like a report for? (YY format) : \n\n");

            int yearInput = UserInput.GetTwoDigitYearFromUser();

            using (var connection = new SqliteConnection(completeConnectionString))
            {
                connection.Open();

                var reportCmd = connection.CreateCommand();
                reportCmd.CommandText =
                    @"SELECT COUNT(*) 
                    FROM hours_played 
                    WHERE substr(Date, 7, 2) = @year";  // Extracts the last 2 characters of the year

                reportCmd.Parameters.AddWithValue("@year", (yearInput % 100).ToString("00"));  // Format as two digits

                codingSessionCount = Convert.ToInt32(reportCmd.ExecuteScalar());

                connection.Close();
            }

            Console.Clear();
            Console.WriteLine($"The habit was performed {codingSessionCount} times in {yearInput}.");
        }

        private static string GenerateRandomDate(Random random)
        {
            DateTime startDate = DateTime.Now.AddYears(-1); // Start date: 1 year ago
            int range = (DateTime.Now - startDate).Days;
            return startDate.AddDays(random.Next(range)).ToString("dd-MM-yy");
        }

        internal static void StartNewSession(bool _start)
        {
            if (_start)
            {
                SessionStartTime = DateTime.Now;
            }
            else
            {
                SessionEndTime = DateTime.Now;

                using (var connection = new SqliteConnection(completeConnectionString))
                {
                    string sql = "INSERT INTO hours_played (Date, StartTime, EndTime) VALUES (@Date, @StartTime, @EndTime)";

                    if (SessionStartTime == null)
                    {
                        SessionStartTime = DateTime.Now;
                    }

                    connection.Execute(sql, new
                    {
                        Date = SessionStartTime.Value.Date,
                        StartTime = int.Parse(SessionStartTime.Value.ToString("HHmm")),
                        EndTime = int.Parse(SessionEndTime.Value.ToString("HHmm"))
                    });
                }
            }

            Console.Clear();
        }
    }
}