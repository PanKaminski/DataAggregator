namespace DataAggregator.WebApi.Models
{
    public class ApiTaskResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ApiAggregatorViewModel Api { get; set; }

        public string CronTimeExpression { get; set; }

    }
}
