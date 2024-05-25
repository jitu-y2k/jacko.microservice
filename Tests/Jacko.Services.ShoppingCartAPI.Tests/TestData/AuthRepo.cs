using System;
using System.Text;
using Jacko.Services.ShoppingCartAPI.Models.Dto;
using Jacko.Services.ShoppingCartAPI.Tests.Models.Dto;
using Newtonsoft.Json;

namespace Jacko.Services.ShoppingCartAPI.Tests.TestData
{
	public static class AuthRepo
	{
		private static string? _token = null;
		private static string? _userId = null;
		private static string? _password = null;

		public static async Task<string> GetToken(string userId, string password) {
			if ((_userId != null && _userId !=userId) || (_password != null && _password != password))
			{
				_token = null;
			}

			if (_token == null)
			{
                //Set coupon code
                HttpClient httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:8083") };

				var user = new LoginRequestDto {UserName= userId, Password = password };

				var jsonContent = JsonConvert.SerializeObject(user);

				var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var result = await httpClient.PostAsync("/api/auth/login", stringContent);

                result.EnsureSuccessStatusCode();

                var response = result.Content.ReadAsStringAsync().Result;

                var responseDto = JsonConvert.DeserializeObject<ResponseDto>(response);

                LoginResponseDto loginResponse;

                if (responseDto.IsSuccess)
                {
                    loginResponse = JsonConvert.DeserializeObject<LoginResponseDto>(responseDto.Result.ToString());
                }
                else
                {
                    loginResponse = new LoginResponseDto();
                }
				_token = loginResponse.Token;

				_userId = userId;
				_password = password;
            }			

			return _token;
			
		}
		
	}
}

