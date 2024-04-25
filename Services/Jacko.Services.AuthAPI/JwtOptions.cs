using System;
namespace Jacko.Services.AuthAPI
{
	public class JwtOptions
    {
	    public string Secret { get; set; }
	    public string Issuer { get; set; }
	    public string Audience { get; set; }      
	}
}

