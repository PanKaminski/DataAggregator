namespace DataAggregator.WebApi.Models
{
    public class CovidApiTaskRequest : ApiTaskCreateRequest
    {
        public CovidAggregatorViewModel Api { get; set; }
    }
}
