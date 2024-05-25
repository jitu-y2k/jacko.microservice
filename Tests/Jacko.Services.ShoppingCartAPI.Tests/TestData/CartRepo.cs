using System;
using System.Net.Http;
using AutoFixture;
using AutoFixture.AutoMoq;
using Jacko.Services.ShoppingCartAPI.Models.Dto;
using Newtonsoft.Json;

namespace Jacko.Services.ShoppingCartAPI.Tests.TestData
{
	public static class CartRepo
	{
		private static bool _dataGenerated = false;
		private static CartDto _cartDto;

		public static string? UserId = null;

		public static async Task<CartDto> GetCartData()
		{
			if (!_dataGenerated)			
			{
				var fixture = new Fixture().Customize(new AutoMoqCustomization());
				CartHeaderDto cartHeader = fixture.Create<CartHeaderDto>();
                cartHeader.Name = null;
                cartHeader.Phone = null;
                cartHeader.Email = null;
				var cartHeaderId = cartHeader.CartHeaderId;
				UserId = cartHeader.UserId;

				IEnumerable<CartDetailsDto> cartDetails = fixture.CreateMany<CartDetailsDto>(4);

                var authToken = AuthRepo.GetToken("jitu4@gmail.com", "Jitu4@123").Result;

                //Set coupon code
                HttpClient httpClientforCoupon = new HttpClient(); // { BaseAddress = new Uri("http://localhost:8082") };

                //Generate authorization token
                HttpRequestMessage message = new();
                message.Headers.Add("Authorization", $"Bearer {authToken}");
                message.RequestUri = new Uri("http://localhost:8082/api/coupon");
                message.Method = HttpMethod.Get;

                var result = await httpClientforCoupon.SendAsync(message);

                //var result = httpClientforCoupon.GetAsync("/api/coupon").Result;

                result.EnsureSuccessStatusCode();

                var response = result.Content.ReadAsStringAsync().Result;

                var responseDto = JsonConvert.DeserializeObject<ResponseDto>(response);

                List<CouponDto> coupons;

                if (responseDto.IsSuccess)
                {
                    coupons = JsonConvert.DeserializeObject<List<CouponDto>>(responseDto.Result.ToString());
                }
                else
                {
                    coupons = new List<CouponDto>();
                }

                cartHeader.CouponCode = coupons!.First().CouponCode;

                HttpClient httpClientforProduct = new HttpClient { BaseAddress = new Uri("http://localhost:8081") };

                result = httpClientforProduct.GetAsync("/api/product").Result;

                result.EnsureSuccessStatusCode();

                response = result.Content.ReadAsStringAsync().Result;

                responseDto = JsonConvert.DeserializeObject<ResponseDto>(response);

                List<ProductDto> products;

                if (responseDto.IsSuccess)
                {
                    products = JsonConvert.DeserializeObject<List<ProductDto>>(responseDto.Result.ToString());
                }
                else
                {
                    products = new List<ProductDto>();
                }



                var productCounter = 0;

                foreach (var cartItem in cartDetails)
                {
                    cartItem.CartHeaderId = cartHeaderId;
                    cartItem.Product = null;
                    cartItem.CartHeader = null;
                    cartItem.ProductId = products.Skip(productCounter).Take(1).FirstOrDefault()!.ProductId;
                    productCounter++;
                }

                _cartDto = new CartDto()
                {
                    CartHeader = cartHeader,
                    CartDetails = cartDetails
                };

                _dataGenerated = true;
            }
            return _cartDto;
			
		}
	}
}

