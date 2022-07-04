namespace DataAggregator.Dal.Contract.Dtos
{
    public abstract class AggregatorApiDto
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public int ApiTaskKey { get; set; }

        public ApiTaskDto ApiTask { get; set; }
    }
}
