using DataAggregator.Bll.Contract.Models;

namespace DataAggregator.WebApi.Authorization
{
    public interface IJwtUtils
    {
        string GenerateJwtToken(User user);

        int? ValidateJwtToken(string token);
    }
}
