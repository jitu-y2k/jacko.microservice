using System;
using Jacko.Services.OrderAPI.Models.Dto;
using Jacko.Services.OrderAPI.Service.IService;
using Newtonsoft.Json;

namespace Jacko.Services.OrderAPI.Service
{
	public class ProductService:IProductService
	{
        private readonly IHttpClientFactory _clientFactory;

        public ProductService(IHttpClientFactory clientFactory)
		{
            _clientFactory = clientFactory;
        }

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            var httpClient = _clientFactory.CreateClient("Product");

            var response = await httpClient.GetAsync($"/api/product");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var output = JsonConvert.DeserializeObject<ResponseDto>(content);

                if (output.IsSuccess)
                {
                    return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(Convert.ToString(output.Result));
                }

            }

            return new List<ProductDto>();
        }
    }
}

