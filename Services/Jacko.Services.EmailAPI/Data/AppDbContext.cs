using System;
using Jacko.Services.EmailAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Jacko.Services.EmailAPI.Data
{
	public class AppDbContext:DbContext
	{

		public AppDbContext(DbContextOptions<AppDbContext> dbContext)
			:base(dbContext)
		{
		}

		public DbSet<EmailLogger> EmailLoggers { get; set; }

        
    }
}

