using System;
using Jacko.Services.RewardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Jacko.Services.RewardAPI.Data
{
	public class AppDbContext:DbContext
	{

		public AppDbContext(DbContextOptions<AppDbContext> dbContext)
			:base(dbContext)
		{
		}

		public DbSet<Rewards> Rewards { get; set; }        


    }
}

