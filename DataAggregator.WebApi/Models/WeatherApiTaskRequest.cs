namespace DataAggregator.WebApi.Models
{
    public class WeatherApiTaskRequest : ApiTaskCreateRequest
    {
        public WeatherAggregatorViewModel Api { get; set; }
    }
}
