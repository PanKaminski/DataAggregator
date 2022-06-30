namespace DataAggregator.Bll.Contract.Models
{
    public record MessageDetails(string TargetEmail, string TargetUserName, string Subject, MemoryStream Data);
}
