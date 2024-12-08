/*
This app allows you to track your hours-played in terms of 
computer and video games, to help you make better habits.
*/
namespace coding_tracker
{
    /// <summary>
    /// Responsible for the main app flow
    /// </summary>
    class Program
    {
        internal static bool IsTimingASession = false;     // Sets to true when user starts timing a new session

        static void Main(string[] args)
        {
            DBController.InitialiseDB();
            AppFlow();
        }

        /// <summary>
        /// Handles the core loop of showing the user the main menu and accepting input
        /// </summary>
        static void AppFlow()
        {
            Console.Clear();

            bool quitApp = false;

            while (quitApp == false)
            {
                View.DisplayMainMenuOptions();

                string? command = Console.ReadLine();

                switch (command)
                {
                    case "0":

                        if (IsTimingASession)
                        {
                            DBController.StartNewSession(false);  // End any currently-recording sessions
                        }

                        Console.WriteLine("\nGoodbye!\n");
                        quitApp = true;
                        Environment.Exit(0);
                        break;
                    case "1":
                        View.DisplayAllEntries(DBController.GetAllRecords());
                        break;
                    case "2":
                        DBController.Insert();
                        break;
                    case "3":
                        DBController.Delete();
                        break;
                    case "4":
                        DBController.Update();
                        break;
                    case "5":
                        DBController.GetReportForAYear();
                        break;
                    case "6":
                        IsTimingASession = !IsTimingASession;
                        DBController.StartNewSession(IsTimingASession);
                        break;
                    default:
                        Console.WriteLine("\nInvalid Command. Please type a number from 0 to 4.\n");
                        break;
                }
            }
        }
    }
}