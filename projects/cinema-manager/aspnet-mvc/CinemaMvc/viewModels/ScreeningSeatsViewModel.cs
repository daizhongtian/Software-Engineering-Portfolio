namespace CinemaMvc.ViewModels
{
    public class ScreeningSeatsViewModel
    {
        public int ScreeningId { get; set; }

        public string FilmTitle { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }

        public string CinemaName { get; set; } = string.Empty;

        public int Rows { get; set; }

        public int SeatsPerRow { get; set; }

        public HashSet<string> ReservedSeats { get; set; } = new();

        public HashSet<string> MySeats { get; set; } = new();
    }
}
