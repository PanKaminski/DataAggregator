namespace DataAggregator.WebApi.Models
{
    public class ApiTaskCreateRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public ApiAggregatorViewModel Api { get; set; }

        public string CronTimeExpression { get; set; }
    }
}
