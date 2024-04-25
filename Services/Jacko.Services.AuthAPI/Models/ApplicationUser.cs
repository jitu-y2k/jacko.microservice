using System;
using Microsoft.AspNetCore.Identity;

namespace Jacko.Services.AuthAPI.Models
{
	public class ApplicationUser: IdentityUser
	{
		public string Name { get; set; }
	}
}

