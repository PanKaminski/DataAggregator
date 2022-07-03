using System.Text.Json.Serialization;

namespace DataAggregator.Bll.Contract.Models
{
    public class User
    {
        public int Id { get; set; }
         
        public string Email { get; set; }

        public UserRole Role { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; set; }

        public IEnumerable<ApiTask> ApiSubscriptions { get; set; }
    }
}
