namespace CinemaMvc.Dtos;

public class SeatDto
{
    public int RowNumber { get; set; }
    public int SeatNumber { get; set; }
    public bool IsReserved { get; set; }
    public bool IsMine { get; set; }
}

public class ScreeningSeatsDto
{
    public int ScreeningId { get; set; }
    public string FilmTitle { get; set; } = "";
    public DateTime StartTime { get; set; }
    public string CinemaName { get; set; } = "";
    public int Rows { get; set; }
    public int SeatsPerRow { get; set; }
    public List<SeatDto> Seats { get; set; } = [];
}


public class ReserveSeatDto
{
    public int RowNumber { get; set; }
    public int SeatNumber { get; set; }
}
