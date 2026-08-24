namespace CinemaMvc.Dtos;

public class CurrentUserDto
{
    public bool IsAuthenticated { get; set; }
    public string? Email { get; set; }
    public string[] Roles { get; set; } = [];
}

public class RegisterDto
{
    public string Email { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string? PhoneNumber { get; set; }
    public string Password { get; set; } = "";
    public string ConfirmPassword { get; set; } = "";
}

public class LoginDto
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public bool RememberMe { get; set; }
}
