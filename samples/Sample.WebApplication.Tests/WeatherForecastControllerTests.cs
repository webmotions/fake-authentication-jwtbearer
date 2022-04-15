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
                        
                            collection.AddAuthenticationCore(x => x.DefaultAuthenticateScheme = FakeJwtBearerDefaults.AuthenticationScheme);
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
        public async Task root_endpoint_should_return_authorized_when_jwt_is_not_set()
        {
            await _host.StartAsync();
            var response = await _host.GetTestServer().CreateClient().GetAsync("/api/weatherforecast");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        public void Dispose()
        {
            _host?.Dispose();
        }
    }
}
