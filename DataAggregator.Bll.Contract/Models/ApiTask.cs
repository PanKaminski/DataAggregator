namespace DataAggregator.Bll.Contract.Models
{
    public class ApiTask
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public AggregatorApi Api { get; set; }

        public User Subscriber { get; set; }

        public string CronTimeExpression { get; set; }
    }
}
