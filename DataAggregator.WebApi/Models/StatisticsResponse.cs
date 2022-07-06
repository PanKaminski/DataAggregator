namespace DataAggregator.WebApi.Models
{
    public class StatisticsResponse
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public int CountOfRequests { get; set; }

        public DateTime RegistrationDate { get; init; }

    }
}
