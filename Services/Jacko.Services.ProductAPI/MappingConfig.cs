using System;
using AutoMapper;
using Jacko.Services.ProductAPI.Models;
using Jacko.Services.ProductAPI.Models.Dto;

namespace Jacko.Services.ProductAPI
{
	public class MappingConfig
	{
		public static MapperConfiguration RegisterMaps()
		{
			MapperConfiguration mapperConfiguration = new(config =>
			{
				config.CreateMap<ProductDto, Product>().ReverseMap();
			});

			return mapperConfiguration;
		}
	}
}

