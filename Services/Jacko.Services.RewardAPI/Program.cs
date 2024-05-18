using Jacko.Services.RewardAPI.Data;
using Jacko.Services.RewardAPI.Extension;
using Jacko.Services.RewardAPI.Messaging;
using Jacko.Services.RewardAPI.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var optionBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Services.AddSingleton(new RewardService(optionBuilder.Options));

if (builder.Configuration.GetValue<string>("AsyncCommunicationMode").ToLower() == "rabbitmq")
{
    builder.Services.AddHostedService<RabbitMQRewardsConsumer>();
}
else
{
    builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();
}


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

if (app.Configuration.GetValue<string>("AsyncCommunicationMode").ToLower() != "rabbitmq")
{
    app.UseAzureServiceBusConsumer();
}

app.Run();

void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (_db.Database.GetPendingMigrations().Count() > 0)
        {
            _db.Database.Migrate();
        }
    }
}

