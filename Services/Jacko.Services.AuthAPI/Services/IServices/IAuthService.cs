using System;
using Jacko.Services.AuthAPI.Models.Dto;

namespace Jacko.Services.AuthAPI.Services.IServices
{
	public interface IAuthService
	{
        Task<string> Register(RegistrationRequestDto registrationRequestDto);
		Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<bool> AssignRole(string email, string roleName);
    }
}

