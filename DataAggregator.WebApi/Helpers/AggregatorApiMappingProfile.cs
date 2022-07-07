﻿using AutoMapper;
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
            // user dto <-> model
            this.CreateMap<UserDto, User>().ForMember(dm => dm.Role,
                expression => expression.MapFrom(me => (UserRole)(byte)me.Role));
            this.CreateMap<User, UserDto>().ForMember(dm => dm.Role,
                expression => expression.MapFrom(me => (UserRoleDto)(byte)me.Role));

            // api task dto <-> model
            this.CreateMap<ApiTaskDto, ApiTask>().ReverseMap();

            // api task aggregators bll dto -> model
            this.CreateMap<AggregatorApiDto, AggregatorApi>()
                .Include<CoinRankingApiDto, CoinRankingApi>()
                .Include<WeatherApiDto, WeatherApi>()
                .Include<CovidAggregatorApiDto, CovidAggregatorApi>();
            this.CreateMap<CoinRankingApiDto, CoinRankingApi>();
            this.CreateMap<WeatherApiDto, WeatherApi>();
            this.CreateMap<CovidAggregatorApiDto, CovidAggregatorApi>();

            // api task aggregators bll model -> dto
            this.CreateMap<AggregatorApi, AggregatorApiDto>()
                .Include<CoinRankingApi, AggregatorApiDto>()
                .Include<WeatherApi, AggregatorApiDto>()
                .Include<CovidAggregatorApi, CovidAggregatorApiDto>();
            this.CreateMap<CoinRankingApi, CoinRankingApiDto>()
                .ForMember(dto => dto.ApiType, expr
                    => expr.MapFrom(me => ApiTypeDto.CoinRanking));
            this.CreateMap<WeatherApi, WeatherApiDto>()
                .ForMember(dto => dto.ApiType, expr
                    => expr.MapFrom(me => ApiTypeDto.WeatherTracker));
            this.CreateMap<CovidAggregatorApi, CovidAggregatorApiDto>()
                .ForMember(dto => dto.ApiType, expr
                    => expr.MapFrom(me => ApiTypeDto.CovidTracker));

            // user response
            this.CreateMap<User, StatisticsResponse>();

            // api task response
            this.CreateMap<ApiTask, ApiTaskItemResponse>().ForMember(target => target.CronExpression,
                expr => expr.MapFrom(bll => bll.CronTimeExpression));

            //api task requests
            this.CreateMap<ApiTaskCreateRequest, ApiTask>();
            this.CreateMap<ApiAggregatorViewModel, AggregatorApi>()
                .Include<CoinRankingAggregatorViewModel, CoinRankingApi>()
                .Include<WeatherAggregatorViewModel, WeatherApi>()
                .Include<CoinRankingAggregatorViewModel, CovidAggregatorApi>().ReverseMap();
            this.CreateMap<CoinRankingAggregatorViewModel, CoinRankingApi>().ReverseMap();
            this.CreateMap<WeatherAggregatorViewModel, WeatherApi>().ReverseMap();
            this.CreateMap<CoinRankingAggregatorViewModel, CovidAggregatorApi>().ReverseMap();

            //api task response
            this.CreateMap<ApiTask, ApiTaskResponse>();
        }
    }
}
