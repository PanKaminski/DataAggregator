using System.ComponentModel.DataAnnotations;

namespace DataAggregator.WebApi.Models;

public class AuthenticateRequest
{
    [Required]
    [DataType(DataType.EmailAddress)]
    [StringLength(40, ErrorMessage = "Must be between 5 and 40 characters", MinimumLength = 5)]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [StringLength(30, ErrorMessage = "Must be between 5 and 30 characters", MinimumLength = 5)]
    public string Password { get; set; }
}