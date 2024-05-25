//extern alias ProductAPITypes;
//using ProductAPITypes.Jacko.Services.ProductAPI.Data;
using System;
using System.Data.Common;
//using Jacko.Services.ShoppingCartAPI.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoFixture;
using AutoFixture.AutoMoq;
using Jacko.Services.ShoppingCartAPI.Models;
using Newtonsoft.Json;
using Jacko.Services.ShoppingCartAPI.Models.Dto;
using Jacko.Services.ShoppingCartAPI.Tests.TestData;
using AutoMapper;
using Jacko.Services.ShoppingCartAPI.Utility;
//using Jacko.Services.ShoppingCartAPI.Models;

namespace Jacko.Services.ShoppingCartAPI.Tests
{
    public class CustomWebApplicationFactory<TProgram, TAppDbContext> : WebApplicationFactory<TProgram> where TProgram : class where TAppDbContext: DbContext
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(async services =>
            {
                // Remove the app's DbContext registration.
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<TAppDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbConnection));

                services.Remove(dbConnectionDescriptor);

                // Add a database context (YourDbContext) using an in-memory database for testing.
                services.AddDbContext<TAppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryCartDb");
                });

                services.AddHttpClient("Product", u=> u.BaseAddress = new Uri("http://localhost:8081")).AddHttpMessageHandler<BackEndAPIAuthenticationClientHandler>();
                services.AddHttpClient("Coupon", u=> u.BaseAddress = new Uri("http://localhost:8082")).AddHttpMessageHandler<BackEndAPIAuthenticationClientHandler>();
                services.AddTransient<BackEndAPIAuthenticationClientHandler>();
                //services.AddHttpClient();

                // Build the service provider.
                var serviceProvider = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database context (YourDbContext).
                using (var scope = serviceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<TAppDbContext>();

                    // Ensure the database is created.
                    db.Database.EnsureCreated();

                    // Seed the database with test data.
                    InitializeDbForTests(db);
                }
            });
        }

        private void InitializeDbForTests(TAppDbContext db)
        {
            CartDto cartDto = CartRepo.GetCartData().Result;

            IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();

            db.Add(mapper.Map<CartHeader>(cartDto.CartHeader));
            db.SaveChanges();
            db.AddRange(mapper.Map<IEnumerable<CartDetails>>(cartDto.CartDetails!));
            db.SaveChanges();
        }

    }
}




