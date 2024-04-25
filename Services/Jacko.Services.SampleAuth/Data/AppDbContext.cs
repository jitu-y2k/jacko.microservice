using System;
using Jacko.Services.SampleAuth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Jacko.Services.SampleAuth.Data
{
	public class AppDbContext:IdentityDbContext<ApplicationUser>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
			:base(options)
		{
		}

		public DbSet<ApplicationUser> ApplicationUsers { get; set; }
		
	}


}

