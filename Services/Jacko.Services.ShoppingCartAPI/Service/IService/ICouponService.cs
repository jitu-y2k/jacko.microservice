using Jacko.Services.ShoppingCartAPI.Models.Dto;

namespace Jacko.Services.ShoppingCartAPI.Service.IService
{
    public interface ICouponService
    {
        Task<CouponDto> GetCoupon(string couponCode);
    }
}
