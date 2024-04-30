using System;
using Jacko.Services.OrderAPI.Models.Dto;

namespace Jacko.Services.OrderAPI.Service.IService
{
	public interface IProductService
	{
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}

