using Jacko.Services.ShoppingCartAPI.Models.Dto;

namespace Jacko.Services.ShoppingCartAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
