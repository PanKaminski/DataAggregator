namespace DataAggregator.Bll.Contract.Models
{
    public interface IAggregatorApi
    {
        public int Id { get; set; }

        public ApiType Name { get; }

        public string Description { get; set; }

        public string ApiUrl { get; set; }
    }
}
