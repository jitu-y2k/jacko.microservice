using System;
using Microsoft.AspNetCore.Identity;

namespace Jacko.Services.SampleAuth.Models
{
	public class ApplicationUser:IdentityUser
	{
		public string Name { get; set; }
	}
}

