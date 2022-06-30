namespace DataAggregator.Bll.Contract.Interfaces
{
    public interface IEmailDataSender
    {
        void SendDataOnEmail(string email, MemoryStream data);
    }
}
