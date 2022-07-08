namespace DataAggregator.WebApi.Models
{
    public class ApiTaskItemResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string CronExpression { get; set; }
    }
}
