using AutoFixture;
using Moq;
using FluentAssertions;
using Jacko.Services.ProductAPI.Data;
using Jacko.Services.ProductAPI.Controllers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Jacko.Services.ProductAPI.Models;
using Jacko.Services.ProductAPI.Models.Dto;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Jacko.Services.ProductAPI.Tests;

public class UnitTest1: IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly IFixture _fixture;
    private readonly Mock<AppDbContext> _mockContext;
    private readonly Mock<DbSet<Product>> _mockSet;
    private readonly ProductController _productController;
    private readonly IMapper _mapper;
    private readonly HttpClient _httpClient;

    public UnitTest1(CustomWebApplicationFactory<Program> factory)
    {
        _mapper = MappingConfig.RegisterMaps().CreateMapper();

        _fixture = new Fixture().Customize(new AutoMoqCustomization());

        _httpClient = factory.CreateClient();

        // Set up the in-memory DbContextOptions
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryProductDb")
            .Options;

        _mockContext = new Mock<AppDbContext>(options);

        // Freeze _mockContext to use the same instance
        _fixture.Inject(_mockContext);

        //var products = _fixture.CreateMany<Product>().AsQueryable();
        var products = new List<Product>();

        _mockSet = CreateMockDbSet(products.AsQueryable());

        //_mockSet = _fixture.Freeze<Mock<DbSet<Product>>>();
        //_mockContext = _fixture.Freeze<Mock<AppDbContext>>();

        _mockContext.Setup(x => x.Products).Returns(_mockSet.Object);

        _productController = new ProductController(_mockContext.Object, _mapper);

    }

    private Mock<DbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class
    {
        var mockSet = new Mock<DbSet<T>>();
        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        mockSet.As<IEnumerable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        return mockSet;
    }

    [Fact]    
    public void GetProducts_ShouldReturnResponseDtoWithIsSuccessTrue_WhenDataFound()
    {
        //Arange       

        var products = _fixture.CreateMany<Product>(5).ToList();
        var mockSet = CreateMockDbSet(products.AsQueryable());
        _mockContext.Setup(c => c.Products).Returns(mockSet.Object);

        //Act
        var result = _productController.Get();

        //Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<ResponseDto>();
        ResponseDto responseDto = result as ResponseDto;
        responseDto.IsSuccess.Should().BeTrue();
        responseDto.Message.Equals("");
        responseDto.Result.As<List<ProductDto>>().Count().Should().Be(5);

    }

    [Fact]
    public void GetProducts_ShouldReturnResponseDtoWithIsSuccessTrueWithEmptyProductList_WhenDataNotFound()
    {
        //Arange       

        //var products = _fixture.CreateMany<Product>(5).ToList();
        //var mockSet = CreateMockDbSet(products.AsQueryable());
        //_mockContext.Setup(c => c.Products).Returns(mockSet.Object);

        //Act
        var result = _productController.Get();

        //Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<ResponseDto>();
        ResponseDto responseDto = result as ResponseDto;
        responseDto.IsSuccess.Should().BeTrue();
        responseDto.Message.Should().BeEmpty();
        responseDto.Result.As<List<ProductDto>>().Count().Should().Be(0);

    }[Fact]
    public async Task CreateProduct_ShouldReturnResponseDtoWithIsSuccessTrue_WhenNewProductCreated()
    {
        //Arange       

        var productToCreate = _fixture.Create<ProductDto>();
        productToCreate.ImageLocalPath = null;
        productToCreate.ImageUrl = null;
        productToCreate.Image = null;

        var jsonContent = JsonConvert.SerializeObject(productToCreate, Formatting.Indented, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });

        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        //Act
        var response = await _httpClient.PostAsync("/api/product", content);

        //Assert
        response.EnsureSuccessStatusCode();

        var result = response.Content.ReadAsStringAsync();

        var createdProductResponse = JsonConvert.DeserializeObject<ResponseDto>(result.Result);

        createdProductResponse.Should().NotBeNull();
        createdProductResponse!.IsSuccess.Should().BeTrue();
        createdProductResponse.Message.Should().BeEmpty();

        var createdProduct = JsonConvert.DeserializeObject<ProductDto>(createdProductResponse.Result.ToString());

        createdProduct.ProductId.Should().Be(productToCreate.ProductId);



    }
}
