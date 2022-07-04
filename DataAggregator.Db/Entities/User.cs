using System.Text.Json.Serialization;

namespace DataAggregator.Db.Entities
{
    public class User
    {
        public int Id { get; set; }
         
        public string Email { get; set; }

        public UserRole Role { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; set; }

        public int CountOfRequests { get; set; }

        public DateTime RegistrationDate { get; init; }

        public IEnumerable<ApiTask> ApiSubscriptions { get; set; }
    }
}
