using System;
using AutoMapper;
using Jacko.Services.OrderAPI.Models;
using Jacko.Services.OrderAPI.Models.Dto;

namespace Jacko.Services.OrderAPI
{
	public class MappingConfig
	{
		public static MapperConfiguration RegisterMaps()
		{
			MapperConfiguration mapperConfiguration = new MapperConfiguration(config =>
			{
                config.CreateMap<OrderHeaderDto, CartHeaderDto>()
                .ForMember(dest => dest.CartTotal, u => u.MapFrom(src => src.OrderTotal)).ReverseMap();

                config.CreateMap<CartDetailsDto, OrderDetailsDto>()
                .ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Price, u => u.MapFrom(src => src.Product.Price));

                config.CreateMap<OrderDetailsDto, CartDetailsDto>();

                config.CreateMap<OrderHeader, OrderHeaderDto>().ReverseMap();
                config.CreateMap<OrderDetailsDto, OrderDetails>().ReverseMap();


            });
			return mapperConfiguration;
		}
	}
}

