namespace CinemaMvc.Dtos;

public class CinemaDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Rows { get; set; }
    public int SeatsPerRow { get; set; }
}