namespace coding_tracker
{
    public class CodingSession
    {
        public required int Id { get; set; }
        public required DateTime Date { get; set; }
        public required int StartTime { get; set; }
        public required int EndTime { get; set; }

        public int Duration  // In minutes
        {
            get
            {
                int startMinutes = (StartTime / 100) * 60 + (StartTime % 100);
                int endMinutes = (EndTime / 100) * 60 + (EndTime % 100);
                int durationMinutes = endMinutes - startMinutes;

                // Handle cases where endTime is on the next day
                if (durationMinutes < 0)
                {
                    durationMinutes += 24 * 60;
                }

                return durationMinutes;
            }
        }
    }
}