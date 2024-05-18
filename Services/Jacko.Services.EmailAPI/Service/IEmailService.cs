using System;
using Jacko.Services.EmailAPI.Message;
using Jacko.Services.EmailAPI.Models.Dto;

namespace Jacko.Services.EmailAPI.Service
{
	public interface IEmailService
	{
		Task EmailCartAndLog(CartDto cartDto);
        Task RegisterUserEmailAndLog(string email);
        Task LogOrderPlaced(RewardsMessage rewardsDto);
    }
}

