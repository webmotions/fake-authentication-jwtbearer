# Fake Authentication Jwt Bearer for ASP.NET Core

This code allow to fake a Jwt Bearer and build integration test for ASP.Net Core application.  
By this way we can fake any authentication we need, without the need to really authenticate a user.  
This code is based on [Microsoft.AspNetCore.Authentication.JwtBearer](https://github.com/aspnet/AspNetCore/tree/master/src/Security/Authentication/JwtBearer) and was forked from [GST.Fake.Authentication.JwtBearer](https://github.com/GestionSystemesTelecom/fake-authentication-jwtbearer).

 > If You need it for ASP.NET Core 1, check [Tag 1.0.4](https://github.com/DOMZE/fake-authentication-jwtbearer/tree/1.0.4)

 > If You need it for ASP.NET Core 2.1, check [Tag 2.1.2](https://github.com/DOMZE/fake-authentication-jwtbearer/tree/2.1.2)

 > If you need it for ASP.NET Core 2.2, check [Tag 2.2.0](https://github.com/DOMZE/fake-authentication-jwtbearer/tree/2.2.0)

 > If you need it for ASP.NET Core 3.1, check [Tag 3.1.1](https://github.com/DOMZE/fake-authentication-jwtbearer/tree/3.1.1)

 > If you need it for ASP.NET Core 5.0, check [Tag 5.1.0](https://github.com/DOMZE/fake-authentication-jwtbearer/tree/5.1.0)

 > If you need it for ASP.NET Core 6.0, check [Tag 6.1.1](https://github.com/DOMZE/fake-authentication-jwtbearer/tree/6.1.1)

 > If you need it for ASP.NET Core 7.0, check [Tag 7.0.0](https://github.com/DOMZE/fake-authentication-jwtbearer/tree/7.0.0)

  > If you need it for ASP.NET Core 8.0, check [Tag 8.0.2](https://github.com/DOMZE/fake-authentication-jwtbearer/tree/8.0.2)

**NOTE**: Version 4.0 was skipped to follow Microsoft versioning pattern for .NET

## How to install it?

Install the package [WebMotions.Fake.Authentication.JwtBearer](https://www.nuget.org/packages/WebMotions.Fake.Authentication.JwtBearer)
<br/>**OR**<br/>
Clone and reference the project Fake.Authentication.JwtBearer under the src folder in your test(s) project(s)

## How to use it?

Now all the things are tied up, how to fake a user?

I've defined three tests methods :
 - One that verifies a token through an Expando object
 - One that verifies a token through a dictionary of claims
 - One that fails if the token is not set

 All of the below can be found under the samples folder

```C#
using System;
using System.Dynamic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebMotions.Fake.Authentication.JwtBearer;
using Xunit;

namespace Sample.WebApplication.Tests
{
    public class WeatherForecastControllerTests : IDisposable
    {
        private readonly IHost _host;

        public WeatherForecastControllerTests()
        {
            _host = new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder
                        .UseTestServer()
                        .ConfigureTestServices(collection =>
                        {
                            collection.AddAuthentication(FakeJwtBearerDefaults.AuthenticationScheme).AddFakeJwtBearer();
                        });
                }).Build();
        }

        [Fact]
        public async Task root_endpoint_should_not_return_authorized_when_jwt_is_set()
        {
            await _host.StartAsync();
            dynamic data = new ExpandoObject();
            data.sub = Guid.NewGuid();
            data.role = new [] {"sub_role","admin"};

            var httpClient = _host.GetTestServer().CreateClient();
            httpClient.SetFakeBearerToken((object)data);

            var response = await httpClient.GetAsync("/api/weatherforecast");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task root_endpoint_should_return_unauthorized_when_jwt_is_not_set()
        {
            await _host.StartAsync();
            var response = await _host.GetTestServer().CreateClient().GetAsync("/api/weatherforecast");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task root_endpoint_should_authorized_when_jwt_is_set_with_using_claims_dictionary()
        {
            await _host.StartAsync();
            var claims = new Dictionary<string, object>
            {
                { ClaimTypes.Name, "test@sample.com" },
                { ClaimTypes.Role, "admin" },
                { "http://mycompany.com/customClaim", "someValue" },
            };
            var httpClient = _host.GetTestServer().CreateClient();
            httpClient.SetFakeBearerToken(claims);
            var response = await httpClient.GetAsync("/api/weatherforecast");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public void Dispose()
        {
            _host?.Dispose();
        }
    }
}
```
