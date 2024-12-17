using System.Globalization;
using Microsoft.Data.Sqlite;
using Spectre.Console;

namespace coding_tracker
{
    /// <summary>
    /// Responsible for showing the UI to the user
    /// </summary>
    class View
    {
        internal static void DisplayMainMenuOptions()
        {
            var table = new Table();
            table.AddColumn("Welcome to the game-tracking app Main Menu \nWhat would you like to do? \n");
            table.AddRow("1. View all records");
            table.AddRow("2. Insert a record");
            table.AddRow("3. Delete a record");
            table.AddRow("4. Update a record");
            table.AddRow("5. Get report for a year");

            if (Program.IsTimingASession)
            {
                table.AddRow($"6. Stop the current session (Started {DBController.GetSessionStartTime()})");
            }
            else
            {
                table.AddRow("6. Start a new session now.");
            }

            table.AddRow("0. Exit");

            AnsiConsole.Write(table);
        }

        internal static void DisplayAllEntries(List<CodingSession> _tableData)
        {
            var table = new Table();
            table.AddColumn("All entries:");

            foreach (var dw in _tableData)
            {
                string formattedStartTime = dw.StartTime.ToString("D4");  // Format to 4 digits with leading zero
                string formattedEndTime = dw.EndTime.ToString("D4");

                table.AddRow($"{dw.Id} - {dw.Date.ToString("dd-MM-yy")} - S: {formattedStartTime}, E: {formattedEndTime}, Duration: {dw.Duration} minutes");
            }

            AnsiConsole.Write(table);
        }
    }
}