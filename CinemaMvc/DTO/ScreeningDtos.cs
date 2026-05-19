namespace CinemaMvc.Dtos;

public class ScreeningDto
{
    public int Id { get; set; }
    public string FilmTitle { get; set; } = "";
    public DateTime StartTime { get; set; }
    public int CinemaId { get; set; }
    public string CinemaName { get; set; } = "";
}

public class CreateScreeningDto
{
    public string FilmTitle { get; set; } = "";
    public DateTime StartTime { get; set; }
    public int CinemaId { get; set; }
}