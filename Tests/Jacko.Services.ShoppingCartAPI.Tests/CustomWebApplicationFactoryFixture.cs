//Enable for ShoppingCart API
using Jacko.Services.ShoppingCartAPI.Data;

using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
    
namespace Jacko.Services.ShoppingCartAPI.Tests
{

    public class CustomWebApplicationFactoryFixture : IDisposable
    {
        public CustomWebApplicationFactory<Program, AppDbContext> Factory { get; }

        public CustomWebApplicationFactoryFixture()
        {
            Factory = new CustomWebApplicationFactory<Program, AppDbContext>();
        }

        public void Dispose()
        {
            // Cleanup if necessary
            Factory.Dispose();
        }
    }

}

