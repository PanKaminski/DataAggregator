namespace DataAggregator.Db.Entities
{
    public class ApiTaskEntity
    {
        public int Id { get; set; }

        public AggregatorApiEntity Api { get; set; }

        public UserEntity Subscriber { get; set; }

        public string CronTimeExpression { get; set; }
    }
}
