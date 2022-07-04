namespace DataAggregator.Db.Entities
{
    public class AggregatorApiEntity
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public int ApiTaskKey { get; set; }

        public ApiTaskEntity ApiTask { get; set; }
    }
}
