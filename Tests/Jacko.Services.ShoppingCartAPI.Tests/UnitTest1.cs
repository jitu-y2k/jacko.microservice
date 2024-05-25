using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Jacko.Services.ShoppingCartAPI.Controllers;
using Jacko.Services.ShoppingCartAPI.Data;
using Jacko.Services.ShoppingCartAPI.Models;
using Jacko.Services.ShoppingCartAPI.Service.IService;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using System.Text;
using Jacko.MessageBus;
using Microsoft.Extensions.Configuration;
using Jacko.Services.ShoppingCartAPI.Models.Dto;
using FluentAssertions;


namespace Jacko.Services.ShoppingCartAPI.Tests;

public class UnitTest1 : IClassFixture<CustomWebApplicationFactory<Program, AppDbContext>>
{
    private readonly IFixture _fixture;
    private readonly Mock<AppDbContext> _mockContext;
    private Mock<DbSet<CartHeader>> _mockSetCartHeader;
    private Mock<DbSet<CartDetails>> _mockSetCartDetails;
    //private readonly Mock<List<ProductDto>> _mockSetProducts;
    //private readonly Mock<List<CouponDto>> _mockSetCoupons;
    private readonly Mock<IProductService> _productService;
    private readonly Mock<ICouponService> _couponService;
    private readonly Mock<IMessageBus> _messageBus;
    private readonly Mock<IConfiguration> _configuration;
    private readonly CartAPIController _cartController;
    private readonly IMapper _mapper;
    private readonly HttpClient _httpClient;

    public UnitTest1(CustomWebApplicationFactory<Program, AppDbContext> factory)
    {
        _mapper = MappingConfig.RegisterMaps().CreateMapper();

        _fixture = new Fixture().Customize(new AutoMoqCustomization());

        _httpClient = factory.CreateClient();

        // Set up the in-memory DbContextOptions
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryCartDb")
            .Options;

        _productService = _fixture.Freeze<Mock<IProductService>>();
        _couponService = _fixture.Freeze<Mock<ICouponService>>();
        _messageBus = _fixture.Freeze<Mock<IMessageBus>>();
        _configuration = _fixture.Freeze<Mock<IConfiguration>>();

        //var products = _fixture.Create<IEnumerable<ProductDto>>();
        //var coupon = _fixture.Create<CouponDto>();

        //_productService.Setup(x => x.GetProducts()).ReturnsAsync(products);
        //_couponService.Setup(x => x.GetCoupon("1")).ReturnsAsync(coupon);

        _mockContext = new Mock<AppDbContext>(options);

        // Freeze _mockContext to use the same instance
        _fixture.Inject(_mockContext);

        //var cartHeaders = _fixture.CreateMany<Product>().AsQueryable();
        var cartHeaders = new List<CartHeader>();

        _mockSetCartHeader = CreateMockDbSet(cartHeaders.AsQueryable());

        //_mockSet = _fixture.Freeze<Mock<DbSet<Product>>>();
        //_mockContext = _fixture.Freeze<Mock<AppDbContext>>();

        _mockContext.Setup(x => x.CartHeaders).Returns(_mockSetCartHeader.Object);

        _cartController = new CartAPIController(_mockContext.Object, _mapper, _productService.Object, _couponService.Object, _messageBus.Object, _configuration.Object);

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
    public async Task GetCartForUser_ShouldReturnResponseDtoWithIsSuccessTrue_WhenDataFound()
    {
        //Arange
        //Create test data for product and coupon

        var products = _fixture.CreateMany<ProductDto>(5);
        var coupon = _fixture.Create<CouponDto>();

        _productService.Setup(x => x.GetProducts()).ReturnsAsync(products);
        _couponService.Setup(x => x.GetCoupon(coupon.CouponCode)).ReturnsAsync(coupon);

        var cartHeaders = _fixture.CreateMany<CartHeader>(1).AsQueryable();
        _mockSetCartHeader = CreateMockDbSet(cartHeaders);
        var cartHeader = cartHeaders.FirstOrDefault();
        var cartHeaderId = cartHeader!.CartHeaderId;
        var userId = cartHeader!.UserId;

        var cartDetails = _fixture.CreateMany<CartDetails>(4).AsQueryable();

        var productCounter = 0;

        foreach (var cartItem in cartDetails)
        {
            cartItem.CartHeaderId = cartHeaderId;
            cartItem.ProductId = products.Skip(productCounter).Take(1).FirstOrDefault()!.ProductId;
            productCounter++;
        }

        _mockSetCartDetails = CreateMockDbSet(cartDetails);
        
        //var mockSet = CreateMockDbSet(cartHeaders.AsQueryable());
        _mockContext.Setup(c => c.CartHeaders).Returns(_mockSetCartHeader.Object);
        _mockContext.Setup(c => c.CartDetails).Returns(_mockSetCartDetails.Object);

        //Act
        var result = await _cartController.GetCart(userId);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<ResponseDto>();
        ResponseDto responseDto = result as ResponseDto;
        responseDto.IsSuccess.Should().BeTrue();
        responseDto.Message.Equals("");
        var cartDto = responseDto.Result.As<CartDto>();

        cartDto.CartHeader.CartHeaderId.Should().Be(cartHeaderId);
        cartDto.CartDetails.Count().Should().Be(4);
        cartDto.CartDetails.Should().BeEquivalentTo(cartDetails.AsEnumerable(), config =>
        {
            config.Excluding(x => x.Product);
            return config;
        });

        cartDto.CartDetails.Select(x => x.ProductId).Should().BeEquivalentTo(cartDetails.Select(x => x.ProductId)); 

    }
    
}
