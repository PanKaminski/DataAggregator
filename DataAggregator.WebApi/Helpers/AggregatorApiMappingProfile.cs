using AutoMapper;
using DataAggregator.Bll.Contract.Models;
using DataAggregator.Dal.Contract.Dtos;
using DataAggregator.Db.Entities;
using DataAggregator.WebApi.Models;

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

            // user dto <-> model
            this.CreateMap<UserDto, User>().ForMember(dm => dm.Role,
                expression => expression.MapFrom(me => (UserRole)(byte)me.Role));
            this.CreateMap<User, UserDto>().ForMember(dm => dm.Role,
                expression => expression.MapFrom(me => (UserRoleDto)(byte)me.Role));

            // api task dto <-> model
            this.CreateMap<ApiTaskDto, ApiTask>().ReverseMap();

            // api task aggregators dto <-> model
            this.CreateMap<AggregatorApiDto, AggregatorApi>()
                .Include<CoinRankingApiDto, AggregatorApi>()
                .Include<WeatherApiDto, WeatherApi>()
                .Include<CovidAggregatorApiDto, AggregatorApi>();
            this.CreateMap<CoinRankingApiDto, AggregatorApi>();
            this.CreateMap<WeatherApiDto, WeatherApi>();
            this.CreateMap<CovidAggregatorApiDto, AggregatorApi>();

            // user response
            this.CreateMap<User, StatisticsResponse>();
        }
    }
}
