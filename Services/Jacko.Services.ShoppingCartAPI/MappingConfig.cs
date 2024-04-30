using System;
using AutoMapper;
using Jacko.Services.ShoppingCartAPI.Models;
using Jacko.Services.ShoppingCartAPI.Models.Dto;

namespace Jacko.Services.ProductAPI
{
	public class MappingConfig
	{
		public static MapperConfiguration RegisterMaps()
		{
			MapperConfiguration mapperConfiguration = new(config =>
			{
				config.CreateMap<CartHeaderDto, CartHeader>().ReverseMap();
				config.CreateMap<CartDetailsDto, CartDetails>().ReverseMap();
			});

			return mapperConfiguration;
		}
	}
}

