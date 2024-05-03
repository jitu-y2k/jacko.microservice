using System;
using Jacko.Services.RewardAPI.Message;

namespace Jacko.Services.RewardAPI.Service.IService
{
	public interface IRewardService
	{
        Task UpdateRewards(RewardsMessage rewardsMessage);
    }
}

