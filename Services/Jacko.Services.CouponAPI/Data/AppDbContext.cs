﻿using System;
using Jacko.Services.CouponAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Jacko.Services.CouponAPI.Data
{
	public class AppDbContext:DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
			:base(options)
		{
		}

		public virtual DbSet<Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Coupon>().HasData(
				new Coupon
				{
					CouponId = 1,
					CouponCode = "10OFF",
					DiscountAmount = 10,
					MinAmount = 20
				},
                new Coupon
                {
                    CouponId = 2,
                    CouponCode = "20OFF",
                    DiscountAmount = 20,
                    MinAmount = 40
                }
                );
        }

    }
}

