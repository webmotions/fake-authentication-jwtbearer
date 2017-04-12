using GST.Fake.Builder;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace GST.Fake.Authentication.JwtBearer.Tests
{
    public class FakeJwtBearerMiddlewareTests
    {
        [Fact]
        public async Task SignInThrows()
        {
            var server = CreateServer(new FakeJwtBearerOptions());
            var transaction = await server.SendAsync("https://example.com/signIn");
            Assert.Equal(HttpStatusCode.OK, transaction.Response.StatusCode);
        }

        [Fact]
        public async Task SignOutThrows()
        {
            var server = CreateServer(new FakeJwtBearerOptions());
            var transaction = await server.SendAsync("https://example.com/signOut");
            Assert.Equal(HttpStatusCode.OK, transaction.Response.StatusCode);
        }

        [Fact]
        public async Task CustomHeaderReceived()
        {
            var server = CreateServer(new FakeJwtBearerOptions());
            
            var response = await server.SendAsync(
                "http://example.com/oauth", 
                "someHeader someblob",
                "Bob",
                new string[] {
                    "Role 1", "Role 2"
                });

            Assert.Equal(
                "FakeBearer {\"sub\":\"Bob\",\"role\":[\"Role 1\",\"Role 2\"]}",
                response.Request.Headers.GetValues("Authorization").FirstOrDefault());
        }

        private static TestServer CreateServer(FakeJwtBearerOptions options)
        {
            return CreateServer(options, handlerBeforeAuth: null);
        }

        private static TestServer CreateServer(FakeJwtBearerOptions options, Func<HttpContext, Func<Task>, Task> handlerBeforeAuth)
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    if (handlerBeforeAuth != null)
                    {
                        app.Use(handlerBeforeAuth);
                    }

                    if (options != null)
                    {
                        app.UseFakeJwtBearerAuthentication(options);
                    }

                    app.Use(async (context, next) =>
                    {
                        if (context.Request.Path == new PathString("/checkforerrors"))
                        {
                            var authContext = new AuthenticateContext(Microsoft.AspNetCore.Http.Authentication.AuthenticationManager.AutomaticScheme);
                            await context.Authentication.AuthenticateAsync(authContext);
                            if (authContext.Error != null)
                            {
                                throw new Exception("Failed to authenticate", authContext.Error);
                            }
                            return;
                        }
                        else if (context.Request.Path == new PathString("/oauth"))
                        {
                            if (context.User == null ||
                                context.User.Identity == null ||
                                !context.User.Identity.IsAuthenticated)
                            {
                                context.Response.StatusCode = 401;

                                return;
                            }

                            var identifier = context.User.FindFirst(ClaimTypes.NameIdentifier);
                            if (identifier == null)
                            {
                                context.Response.StatusCode = 500;

                                return;
                            }

                            await context.Response.WriteAsync(identifier.Value);
                        }
                        else if (context.Request.Path == new PathString("/unauthorized"))
                        {
                            // Simulate Authorization failure 
                            var result = await context.Authentication.AuthenticateAsync(FakeJwtBearerDefaults.AuthenticationScheme);
                            await context.Authentication.ChallengeAsync(FakeJwtBearerDefaults.AuthenticationScheme);
                        }
                        else if (context.Request.Path == new PathString("/signIn"))
                        {
                            await Assert.ThrowsAsync<NotSupportedException>(() => context.Authentication.SignInAsync(FakeJwtBearerDefaults.AuthenticationScheme, new ClaimsPrincipal()));
                        }
                        else if (context.Request.Path == new PathString("/signOut"))
                        {
                            await Assert.ThrowsAsync<NotSupportedException>(() => context.Authentication.SignOutAsync(FakeJwtBearerDefaults.AuthenticationScheme));
                        }
                        else
                        {
                            await next();
                        }
                    });
                })
                .ConfigureServices(services => services.AddAuthentication());

            return new TestServer(builder);
        }
    }
}
