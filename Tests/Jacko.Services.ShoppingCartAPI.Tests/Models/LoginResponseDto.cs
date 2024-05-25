using System;
namespace Jacko.Services.ShoppingCartAPI.Tests.Models.Dto
{
	public class LoginResponseDto
	{
		public UserDto User { get; set; }
		public string Token { get; set; }
	}
}

