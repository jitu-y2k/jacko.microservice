﻿using Jacko.APIGateway.Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.AddAppAuthentication();

if (builder.Environment.IsProduction())
{
    builder.Configuration.AddJsonFile("ocelot.production.json", optional: false, reloadOnChange: true);
}
else
{
    builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
}

builder.Services.AddOcelot(builder.Configuration);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
await app.UseOcelot();
app.Run();

