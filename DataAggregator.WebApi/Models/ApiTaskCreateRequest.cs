namespace DataAggregator.WebApi.Models
{
    public abstract class ApiTaskCreateRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string CronTimeExpression { get; set; }
    }
}
