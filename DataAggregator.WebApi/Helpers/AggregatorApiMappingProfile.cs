using AutoMapper;
using DataAggregator.Dal.Contract.Dtos;
using DataAggregator.Db.Entities;

namespace DataAggregator.WebApi.Helpers
{
    public sealed class AggregatorApiMappingProfile : Profile
    {
        public AggregatorApiMappingProfile()
        {
            this.CreateMap<UserEntity, UserDto>().ReverseMap();
            this.CreateMap<AggregatorApiEntity, AggregatorApiDto>().ReverseMap();
            this.CreateMap<WeatherApiEntity, WeatherApiDto>().ReverseMap();
            this.CreateMap<CovidAggregatorApiEntity, CovidAggregatorApiDto>().ReverseMap();
            this.CreateMap<CoinRankingApiEntity, CoinRankingApiDto>().ReverseMap();
            this.CreateMap<ApiTaskEntity, ApiTaskDto>().ReverseMap();
        }
    }
}
