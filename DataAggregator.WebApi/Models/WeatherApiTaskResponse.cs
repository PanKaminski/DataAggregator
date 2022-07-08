namespace DataAggregator.WebApi.Models
{
    public class WeatherApiTaskResponse : ApiTaskResponse
    {
        public WeatherAggregatorViewModel Api { get; set; }
    }
}
