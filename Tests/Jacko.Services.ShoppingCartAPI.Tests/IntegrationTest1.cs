using FluentAssertions;
using Jacko.Services.ShoppingCartAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using Jacko.Services.ShoppingCartAPI.Tests.TestData;

namespace Jacko.Services.ShoppingCartAPI.Tests
{    
    public class IntegrationTest1:IClassFixture<DockerComposeFixture>
    {       
        private readonly HttpClient _httpClient;

        public IntegrationTest1(DockerComposeFixture fixture)
        {            
            _httpClient = fixture.Factory.CreateClient();            
        }

        [Fact]
        public async Task GetCartForUser_ShouldReturnResponseDtoWithIsSuccessTrue_WhenDataFound()
        {
            //Arange
            //Generate authorization token
            var authToken = AuthRepo.GetToken("jitu4@gmail.com", "Jitu4@123").Result;

            CartDto expectedCart = CartRepo.GetCartData().Result;
            string userId = CartRepo.UserId!;

            //Act
            HttpRequestMessage message = new();            
            message.Headers.Add("Authorization", $"Bearer {authToken}");  
            message.RequestUri = new Uri($"/api/cart/getcart/{userId}");
            message.Method = HttpMethod.Get;

            var result = await _httpClient.SendAsync(message);
                
            result.EnsureSuccessStatusCode();

            //Asert
            var response = await result.Content.ReadAsStringAsync();            

            ResponseDto responseDto = JsonConvert.DeserializeObject<ResponseDto>(response)!;

            responseDto.IsSuccess.Should().BeTrue();

            responseDto.Message.Should().BeEmpty();

            CartDto actualCart = JsonConvert.DeserializeObject<CartDto>(responseDto.Result!.ToString()!)!;

            actualCart.CartHeader.Should().BeEquivalentTo(expectedCart.CartHeader, config =>
            {
                config.Excluding(x => x.CartTotal).Excluding(x => x.Discount);
                return config;
            });

            actualCart.CartDetails.Should().BeEquivalentTo(expectedCart.CartDetails, config =>
            {
                config.Excluding(x => x.Product).Excluding(x => x.CartHeader);
                return config;
            });
            
            
        }

    }
}

