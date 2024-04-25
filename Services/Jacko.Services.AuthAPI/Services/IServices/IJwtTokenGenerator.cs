using System;
using Jacko.Services.AuthAPI.Models;

namespace Jacko.Services.AuthAPI.Services.IServices
{
	public interface IJwtTokenGenerator
	{
		string GenerateToken(ApplicationUser applicationUser, List<string> roles);
	}
}

