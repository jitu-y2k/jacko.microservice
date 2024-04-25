using System;
using Jacko.Services.AuthAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Jacko.Services.AuthAPI.Data
{
	public class AppDbContext: IdentityDbContext<ApplicationUser>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
			:base(options)
		{
		}

		public DbSet<ApplicationUser> ApplicationUsers { get; set; }

	}
}

