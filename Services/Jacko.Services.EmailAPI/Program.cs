﻿using Jacko.Services.EmailAPI.Data;
using Jacko.Services.EmailAPI.Extensions;
using Jacko.Services.EmailAPI.Messaging;
using Jacko.Services.EmailAPI.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var optionBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddSingleton(new EmailService(optionBuilder.Options));

builder.AddAsyncCommunicationService();
//if (builder.Configuration.GetValue<string>("AsyncCommunicationMode").ToLower() == "rabbitmq")
//{
//    builder.Services.AddHostedService<RabbitMQEmailConsumer>();
//}
//else
//{
//    builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();
//}

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

ApplyMigration();

if (app.Configuration.GetValue<string>("AsyncCommunicationConfig:Platform").ToLower() == "azureservicebus") {

    app.UseAzureServiceBusConsumer();
}

app.Run();

void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (dbContext.Database.GetPendingMigrations().Count() > 0)
        {
            dbContext.Database.Migrate();
        }
    }
}


