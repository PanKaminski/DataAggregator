namespace DataAggregator.Bll.Contract.Models
{
    public class ApiTask
    {
        public IAggregatorApi Api { get; set; }

        public User Subscriber { get; set; }

        public string CronTimeExpression { get; set; }
    }
}
