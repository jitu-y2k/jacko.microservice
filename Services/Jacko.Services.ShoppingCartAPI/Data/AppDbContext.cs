using System;
using Jacko.Services.ShoppingCartAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Jacko.Services.ShoppingCartAPI.Data
{
	public class AppDbContext:DbContext
	{

		public AppDbContext(DbContextOptions<AppDbContext> dbContext)
			:base(dbContext)
		{
		}

		public virtual DbSet<CartHeader>  CartHeaders { get; set; }
		public virtual DbSet<CartDetails>  CartDetails { get; set; }

        
    }
}

