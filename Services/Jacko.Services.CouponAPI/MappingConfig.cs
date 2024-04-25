using System;
using AutoMapper;
using Jacko.Services.CouponAPI.Models;
using Jacko.Services.CouponAPI.Models.Dto;

namespace Jacko.Services.CouponAPI
{
	public class MappingConfig
	{
		public static MapperConfiguration RegisterMaps()
		{
			MapperConfiguration mapperConfiguration = new MapperConfiguration(config =>
			{
				config.CreateMap<CouponDto, Coupon>().ReverseMap();
                //config.CreateMap<Coupon, CouponDto>();
                //.ForMember(d=>d.CouponCode, s=>s.MapFrom(sc=>sc.CouponCode))
                //            .ForMember(d => d.DiscountAmount, s => s.MapFrom(sc => sc.DiscountAmount))


            });
			return mapperConfiguration;
		}
	}
}

