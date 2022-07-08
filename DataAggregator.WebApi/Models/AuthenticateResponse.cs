using DataAggregator.Bll.Contract.Models;

namespace DataAggregator.WebApi.Models;

public class AuthenticateResponse
{
    public AuthenticateResponse(User user, string token)
    {
        Id = user.Id;
        Email = user.Email;
        Role = user.Role;
        Token = token;
    }

    public int Id { get; set; }

    public string Email { get; set; }

    public UserRole Role { get; set; }

    public string Token { get; set; }
}