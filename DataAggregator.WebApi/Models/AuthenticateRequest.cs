using System.ComponentModel.DataAnnotations;

namespace DataAggregator.WebApi.Models;

public class AuthenticateRequest
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}