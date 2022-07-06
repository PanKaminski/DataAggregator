using System.ComponentModel.DataAnnotations;

namespace DataAggregator.WebApi.Models
{
    public class RegisterRequest : AuthenticateRequest
    {
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
