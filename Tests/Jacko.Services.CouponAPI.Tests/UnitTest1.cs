using Xunit;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using FluentAssertions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;

using Jacko.Services.CouponAPI.Data;
using Moq;
using Jacko.Services.CouponAPI.Models;
using Jacko.Services.CouponAPI.Controllers;
using Jacko.Services.CouponAPI.Models.Dto;

namespace Jacko.Services.CouponAPI.Tests;

public class UnitTest1
{
    private readonly IMapper _mapper;
    private readonly Mock<AppDbContext> _mockContext;
    private Mock<DbSet<Coupon>> _mockSetCoupons;
    private readonly CouponAPIController _couponController;

    private readonly IFixture _fixture;

    public UnitTest1()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());

        _mapper = MappingConfig.RegisterMaps().CreateMapper();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase("InMemoryCouponData").Options;

        _mockContext = new Mock<AppDbContext>(options);

        _mockSetCoupons = new Mock<DbSet<Coupon>>();

        _fixture.Inject(_mockContext);

        var coupons = new List<Coupon>();
        //var coupons = new List<Coupon>()
        //{
        //    new Coupon
        //        {
        //            CouponId = 1,
        //            CouponCode = "5OFF",
        //            DiscountAmount = 5,
        //            MinAmount = 10
        //        },
        //        new Coupon
        //        {
        //            CouponId = 2,
        //            CouponCode = "15OFF",
        //            DiscountAmount = 15,
        //            MinAmount = 30
        //        }
        //};

        InitializeMockDbSet(_mockSetCoupons, coupons.AsQueryable());

        _mockContext.Setup(x => x.Coupons).Returns(_mockSetCoupons.Object);

        _couponController = new CouponAPIController(_mockContext.Object, _mapper);

        //_fixture.Inject(_couponController);

    }

    private void InitializeMockDbSet<T>(Mock<DbSet<T>> mockSet, IQueryable<T> data) where T : class
    {
        //var mockSet = new Mock<DbSet<T>>();
        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        mockSet.As<IEnumerable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        //return mockSet;
    }


    [Fact]    
    public void GetCoupons_ShouldReturnResponseDto_isSuccess_As_True()
    {
        //Arranging
        //var coupons = new List<Coupon>()
        //{
        //    new Coupon
        //        {
        //            CouponId = 1,
        //            CouponCode = "5OFF",
        //            DiscountAmount = 5,
        //            MinAmount = 10
        //        },
        //        new Coupon
        //        {
        //            CouponId = 2,
        //            CouponCode = "15OFF",
        //            DiscountAmount = 15,
        //            MinAmount = 30
        //        }
        //};

        var coupons = _fixture.CreateMany<Coupon>(4).ToList();

        //_mockSetCoupons = new Mock<DbSet<Coupon>>();

        InitializeMockDbSet(_mockSetCoupons, coupons.AsQueryable());

        //_mockContext.Setup(x => x.Coupons).Returns(_mockSetCoupons.Object);


        //Act
        var result = _couponController.Get();


        //Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<ResponseDto>();

        var response = result as ResponseDto;

        response.IsSuccess.Should().BeTrue();

        response.Message.Should().BeEmpty();

        response.Result.Should().BeAssignableTo<IEnumerable<CouponDto>>();

        var couponList = response.Result as IEnumerable<CouponDto>;

        couponList.Count().Should().Be(coupons.Count());

        couponList.Should().BeEquivalentTo(coupons);

        //couponList.Should().BeEquivalentTo(coupons, config =>
        //{
        //    config.Excluding(x => x.MinAmount).Excluding(x=>x.CouponCode);

        //    return config;
        //});



    }

    [Fact]
    public void GetCoupons_ShouldReturn_CouponCountasZero_WhenNoCouponExists()
    {
        //Arranging
        

        //Act
        var result = _couponController.Get();


        //Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<ResponseDto>();

        var response = result as ResponseDto;

        response.IsSuccess.Should().BeTrue();

        response.Message.Should().BeEmpty();

        response.Result.Should().BeAssignableTo<IEnumerable<CouponDto>>();

        var couponList = response.Result as IEnumerable<CouponDto>;

        couponList.Count().Should().Be(0);

    }

    [Fact]
    public void CreateCoupon_ShouldReturn_NewlyCreatedCoupon()
    {
        //Arranging
        var couponToCreate = _fixture.Create<CouponDto>();


        //Act
        var result = _couponController.Post(couponToCreate);


        //Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<ResponseDto>();

        var response = result as ResponseDto;

        response.IsSuccess.Should().BeTrue();

        response.Message.Should().BeEmpty();

        response.Result.Should().BeAssignableTo<CouponDto>();

        var couponCreated = response.Result as CouponDto;

        couponCreated.Should().BeEquivalentTo(couponToCreate);

    }


    [Fact]
    public void DeleteCoupon_ShouldReturn_ListofCouponsWithOneRemoved()
    {
        //Arranging
        var coupons = _fixture.CreateMany<Coupon>(4).ToList();

        InitializeMockDbSet(_mockSetCoupons, coupons.AsQueryable());
                
        var couponId = coupons.First().CouponId;

        _mockSetCoupons.Setup(x => x.Find(couponId)).Returns(coupons.First(c => c.CouponId == couponId));
        _mockSetCoupons.Setup(x => x.Remove(It.IsAny<Coupon>())).Callback<Coupon>(c=> coupons.Remove(c));
        _mockContext.Setup(x => x.SaveChanges()).Returns(1);        

        //Act
        var result = _couponController.Delete(couponId);


        //Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<ResponseDto>();

        var response = result as ResponseDto;

        response.IsSuccess.Should().BeTrue();

        response.Message.Should().BeEmpty();

        coupons.Count().Should().Be(3);

        coupons.Should().NotContain(x => x.CouponId == couponId);
    }
}
