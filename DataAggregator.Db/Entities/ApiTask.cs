namespace DataAggregator.Db.Entities
{
    public class ApiTask
    {
        public IAggregatorApi Api { get; set; }

        public User Subscriber { get; set; }

        public string CronTimeExpression { get; set; }
    }
}
