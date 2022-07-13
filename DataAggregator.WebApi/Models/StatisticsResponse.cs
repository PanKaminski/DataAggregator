using DataAggregator.Bll.Contract.Models;

namespace DataAggregator.WebApi.Models
{
    public class StatisticsResponse
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public UserRole Role { get; set; }

        public int CountOfRequests { get; set; }

        public int RequestsPerDay { get; init; }

        public DateTime RegistrationDate { get; set; }
    }
}
