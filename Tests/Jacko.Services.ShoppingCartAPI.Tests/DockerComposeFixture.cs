using System;
using System.Diagnostics;
using Jacko.Services.ShoppingCartAPI.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Jacko.Services.ShoppingCartAPI.Tests{

    public class DockerComposeFixture : IDisposable
    {
        private readonly DockerComposeHelper _dockerComposeHelper;
        public CustomWebApplicationFactory<Program, AppDbContext> Factory { get; }

        public DockerComposeFixture()
        {
            
            _dockerComposeHelper = new DockerComposeHelper($"../../../../../docker-compose-test.yml");
            _dockerComposeHelper.Start();


            // Allow some time for the services to start
            Task.Delay(20000).Wait();

            //var factory = new WebApplicationFactory<Program>();
            //Client = factory.CreateClient();
            Factory = new CustomWebApplicationFactory<Program, AppDbContext>();
        }

        public void Dispose()
        {
            _dockerComposeHelper.Dispose();
        }
    }
    

}

