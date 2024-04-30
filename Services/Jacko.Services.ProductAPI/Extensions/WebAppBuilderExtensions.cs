using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Jacko.Services.ProductAPI.Extensions
{
	public static class WebAppBuilderExtensions
	{
		public static WebApplicationBuilder AddAppAuthentication(this WebApplicationBuilder builder)
		{
			var JwtOptions = builder.Configuration.GetSection("ApiSettings:JwtOptions");
			var secret = JwtOptions.GetValue<string>("Secret");
			var issuer = JwtOptions.GetValue<string>("Issuer");
			var audience = JwtOptions.GetValue<string>("Audience");

			builder.Services.AddAuthentication(config =>
			{
				config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(option =>
			{
				option.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
					ValidateIssuer = true,
					ValidIssuer = issuer,
					ValidateAudience = true,
					ValidAudience = audience
				};
			});
			
			return builder;
		}
	}
}

