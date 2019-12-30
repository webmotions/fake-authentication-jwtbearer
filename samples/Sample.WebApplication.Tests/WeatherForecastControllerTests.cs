using System;
using System.Dynamic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using WebMotions.Fake.Authentication.JwtBearer;

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