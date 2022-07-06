namespace DataAggregator.Bll.Contract.Models
{
    public abstract class AggregatorApi
    {
        public int Id { get; set; }

        public ApiTask ApiTask { get; set; }
    }
}
