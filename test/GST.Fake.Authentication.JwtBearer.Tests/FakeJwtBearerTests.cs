using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Microsoft.AspNetCore.Authentication;
using System;
using Microsoft.AspNetCore.Http;
using GST.Fake.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using GST.Fake.Authentication.JwtBearer.Events;
using GST.Fake.Authentication.JwtBearer.Tests.Tools;
using System.Linq;

namespace GST.Fake.Authentication.JwtBearer.Tests
{
    public class FakeJwtBearerTests
    {
        [Fact]
        public async Task VerifySchemeDefaults()
        {
            var services = new ServiceCollection();
            services.AddAuthentication().AddFakeJwtBearer();
            var sp = services.BuildServiceProvider();
            var schemeProvider = sp.GetRequiredService<IAuthenticationSchemeProvider>();
            var scheme = await schemeProvider.GetSchemeAsync(FakeJwtBearerDefaults.AuthenticationScheme);
            Assert.NotNull(scheme);
            Assert.Equal("FakeJwtBearerHandler", scheme.HandlerType.Name);
            Assert.Null(scheme.DisplayName);
        }

        [Fact]
        public async Task CustomHeaderReceived()
        {
            var server = CreateServer(o =>
            {
                o.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        var claims = new[]
                        {
                            new Claim("sub", "Bob le Magnifique"),
                            new Claim(ClaimTypes.Name, "Bob le Magnifique"),
                            new Claim(ClaimTypes.Email, "bob@contoso.com"),
                            new Claim(ClaimsIdentity.DefaultNameClaimType, "bob")
                        };

                        context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));
                        context.Success();

                        return Task.FromResult<object>(null);
                    }
                };
            });

            var response = await SendAsync(server.CreateClient(), "http://example.com/oauth", "someHeader someblob");
            Assert.Equal(HttpStatusCode.OK, response.Response.StatusCode);
            Assert.Equal("Bob le Magnifique", response.ResponseText);
        }

        [Fact]
        public async Task SetRoleReceived()
        {
            var server = CreateServer(o =>
            {
                o.Events = new JwtBearerEvents()
                {
                    OnTokenValidated = context =>
                    {
                        Assert.Contains(context.Principal.Claims, a => a.Type == "role" && a.Value == "Role1");
                        Assert.Contains(context.Principal.Claims, a => a.Type == "role" && a.Value == "Role2");
                        return Task.FromResult<object>(null);
                    }
                };
            });

            var response = await SendAsync(server.CreateClient().SetFakeBearerToken("SUperUserName", new[] { "Role1", "Role2" }), "http://example.com/oauth");
            Assert.Equal(HttpStatusCode.OK, response.Response.StatusCode);
            Assert.Equal("SUperUserName", response.ResponseText);
        }


        [Fact]
        public async Task SetCustomsClaim()
        {
            var server = CreateServer(o =>
            {
                o.Events = new JwtBearerEvents()
                {
                    OnTokenValidated = context =>
                    {
                        Assert.Contains(context.Principal.Claims, a => a.Type == "role" && a.Value == "Role1");
                        Assert.Contains(context.Principal.Claims, a => a.Type == "role" && a.Value == "Role2");
                        Assert.Contains(context.Principal.Claims, a => a.Type == "organism" && a.Value == "ACME");
                        Assert.Contains(context.Principal.Claims, a => a.Type == "thing" && a.Value == "more things");
                        return Task.FromResult<object>(null);
                    }
                };
            });

            dynamic data = new System.Dynamic.ExpandoObject();
            data.organism = "ACME";
            data.thing = "more things";
            var client = server.CreateClient().SetFakeBearerToken("SUperUserName", new[] { "Role1", "Role2" }, (object)data);

            var response = await SendAsync(client, "http://example.com/oauth");
            Assert.Equal(HttpStatusCode.OK, response.Response.StatusCode);
            Assert.Equal("SUperUserName", response.ResponseText);
        }

        /// <summary>
        /// Must fix issue https://github.com/GestionSystemesTelecom/fake-authentication-jwtbearer/issues/2
        /// </summary>
        [Fact]
        public async Task MustFixIssue2Part1()
        {
            var server = CreateServer(o =>
            {
                o.Events = new JwtBearerEvents()
                {
                    OnTokenValidated = context =>
                    {
                        Assert.Contains(context.Principal.Claims, a => a.Type == "name" && a.Value == "Kathy Daugherty");
                        Assert.True(context.Principal.Claims.Where(c => c.Type == "name").ToList().Count() == 1);
                        Assert.Contains(context.Principal.Claims, a => a.Type == "sub" && a.Value == "c611495c-ceb7-0af5-5014-1ecbe067363c");
                        Assert.True(context.Principal.Claims.Where(c => c.Type == "role").ToList().Count() == 2);
                        return Task.FromResult<object>(null);
                    }
                };
            });

            var client = server.CreateClient().SetFakeBearerToken(new
            {
                sub = "c611495c-ceb7-0af5-5014-1ecbe067363c",
                name = "Kathy Daugherty",
                preferred_username = "Kathy Daugherty",
                email = "Kathy_Daugherty1@gmail.com",
                ooperator = "true",
                role = new[]
                {
                "admins",
                "users"
                }
            });

            var response = await SendAsync(client, "http://example.com/oauth");
            Assert.Equal(HttpStatusCode.OK, response.Response.StatusCode);
            Assert.Equal("c611495c-ceb7-0af5-5014-1ecbe067363c", response.ResponseText);
        }

        /// <summary>
        /// Must fix issue https://github.com/GestionSystemesTelecom/fake-authentication-jwtbearer/issues/2
        /// </summary>
        [Fact]
        public async Task MustFixIssue2Part2()
        {
            var server = CreateServer(o =>
            {
                o.Events = new JwtBearerEvents()
                {
                    OnTokenValidated = context =>
                    {
                        Assert.Contains(context.Principal.Claims, a => a.Type == "name" && a.Value == "Earl Becker");
                        Assert.DoesNotContain(context.Principal.Claims, a => a.Type == "sub" && a.Value == "c611495c-ceb7-0af5-5014-1ecbe067363c");
                        Assert.Contains(context.Principal.Claims, a => a.Type == "sub" && a.Value == "Earl Becker");
                        Assert.True(context.Principal.Claims.Where(c => c.Type == "name").ToList().Count() == 1);
                        Assert.True(context.Principal.Claims.Where(c => c.Type == "role").ToList().Count() == 2);
                        return Task.FromResult<object>(null);
                    }
                };
            });

            dynamic data = new System.Dynamic.ExpandoObject();
            data.sub = "801969ed-27f7-c109-af1d-075106644c4b";
            data.name = "Earl Becker";
            data.preferred_username = "Earl Becker";
            data.email = "Earl_Becker@gmail.com";
            data.ooperator = "true";

            var roles = new[]
                    {
                    "admins",
                    "users"
                };

            var client = server.CreateClient().SetFakeBearerToken("Earl Becker", roles, (object)data);

            var response = await SendAsync(client, "http://example.com/oauth");
            Assert.Equal(HttpStatusCode.OK, response.Response.StatusCode);
            Assert.Equal("Earl Becker", response.ResponseText);
        }

        private static TestServer CreateServer(Action<FakeJwtBearerOptions> options = null, Func<HttpContext, Func<Task>, Task> handlerBeforeAuth = null)
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    if (handlerBeforeAuth != null)
                    {
                        app.Use(handlerBeforeAuth);
                    }

                    app.UseAuthentication();
                    app.Use(async (context, next) =>
                    {
                        if (context.Request.Path == new PathString("/checkforerrors"))
                        {
                            var result = await context.AuthenticateAsync(FakeJwtBearerDefaults.AuthenticationScheme); // this used to be "Automatic"
                            if (result.Failure != null)
                            {
                                throw new Exception("Failed to authenticate", result.Failure);
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
                                // REVIEW: no more automatic challenge
                                await context.ChallengeAsync(FakeJwtBearerDefaults.AuthenticationScheme);
                                return;
                            }

                            var identifier = context.User.FindFirst("sub");
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
                            var result = await context.AuthenticateAsync(FakeJwtBearerDefaults.AuthenticationScheme);
                            await context.ChallengeAsync(FakeJwtBearerDefaults.AuthenticationScheme);
                        }
                        else if (context.Request.Path == new PathString("/signIn"))
                        {
                            await Assert.ThrowsAsync<InvalidOperationException>(() => context.SignInAsync(FakeJwtBearerDefaults.AuthenticationScheme, new ClaimsPrincipal()));
                        }
                        else if (context.Request.Path == new PathString("/signOut"))
                        {
                            await Assert.ThrowsAsync<InvalidOperationException>(() => context.SignOutAsync(FakeJwtBearerDefaults.AuthenticationScheme));
                        }
                        else
                        {
                            await next();
                        }
                    });
                })
                .ConfigureServices(services => services.AddAuthentication(FakeJwtBearerDefaults.AuthenticationScheme).AddFakeJwtBearer(options));

            return new TestServer(builder);
        }

        private static async Task<Transaction> SendAsync(HttpClient client, string uri, string authorizationHeader = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                request.Headers.Add("Authorization", authorizationHeader);
            }

            var transaction = new Transaction
            {
                Request = request,
                Response = await client.SendAsync(request),
                //Response = await server.CreateClient().SendAsync(request),
            };

            transaction.ResponseText = await transaction.Response.Content.ReadAsStringAsync();

            if (transaction.Response.Content != null &&
                transaction.Response.Content.Headers.ContentType != null &&
                transaction.Response.Content.Headers.ContentType.MediaType == "text/xml")
            {
                transaction.ResponseElement = XElement.Parse(transaction.ResponseText);
            }

            return transaction;
        }
    }
}
