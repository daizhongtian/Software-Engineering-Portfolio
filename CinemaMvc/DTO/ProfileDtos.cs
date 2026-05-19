namespace CinemaMvc.Dtos;

public class ProfileDto
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string? PhoneNumber { get; set; }
    public string ConcurrencyStamp { get; set; } = "";
}

public class UpdateProfileDto
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string? PhoneNumber { get; set; }
    public string ConcurrencyStamp { get; set; } = "";
}