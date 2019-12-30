# Fake Authentication Jwt Bearer for ASP.NET Core 3.1

This code allow to fake a Jwt Bearer and build integration test for ASP.Net Core application.  
By this way we can fake any authentication we need, without the need to really authenticate a user.  
This code is based on [Microsoft.AspNetCore.Authentication.JwtBearer](https://github.com/aspnet/AspNetCore/tree/master/src/Security/Authentication/JwtBearer) and was forked from [GST.Fake.Authentication.JwtBearer](https://github.com/GestionSystemesTelecom/fake-authentication-jwtbearer).

 > If You need it for ASP.NET Core 1, check [Tag 1.0.4](https://github.com/DOMZE/fake-authentication-jwtbearer/tree/1.0.4)

 > If You need it for ASP.NET Core 2.1, check [Tag 2.1.2](https://github.com/DOMZE/fake-authentication-jwtbearer/tree/2.1.2)

 > If you need it for ASP.NET Core 2.2, check [Tag 2.2.0](https://github.com/DOMZE/fake-authentication-jwtbearer/tree/2.2.0)

## How to install it?

Install the package [WebMotions.Fake.Authentication.JwtBearer](https://www.nuget.org/packages/WebMotions.Fake.Authentication.JwtBearer)
<br/>**OR**<br/>
Clone and reference the project Fake.Authentication.JwtBearer under the src folder in your test(s) project(s)

## How to use it?

Now all the things are tied up, how to fake a user?

I've defined two tests methods :
 - One that verifies a token through an Expando object
 - One that fails if the token is not set

 All of the below can be found under the samples folder

```C#
using System;
using System.Dynamic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Fake.Authentication.JwtBearer;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace Sample.WebApplication.Tests
{
    public class WeatherForecastControllerTests
    {
        private IHost _host;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _host = await new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder.UseStartup<Sample.WebApplication.Startup>();
                    webBuilder
                        .UseTestServer()
                        .ConfigureTestServices(collection =>
                        {
                            collection.AddAuthentication(FakeJwtBearerDefaults.AuthenticationScheme).AddFakeJwtBearer();
                        });
                })
                .StartAsync();
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            _host?.Dispose();
        }

        [Test]
        public async Task root_endpoint_should_not_return_authorized_when_jwt_is_set()
        {
            dynamic data = new ExpandoObject();
            data.sub = Guid.NewGuid();
            data.role = new [] {"sub_role","admin"};

            var httpClient = _host.GetTestServer().CreateClient();
            httpClient.SetFakeBearerToken((object)data);

            var response = await httpClient.GetAsync("/api/weatherforecast");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task root_endpoint_should_return_authorized_when_jwt_is_not_set()
        {
            var response = await _host.GetTestServer().CreateClient().GetAsync("/api/weatherforecast");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
```