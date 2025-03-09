using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using WebMotions.Fake.Authentication.JwtBearer.Events;
using WebMotions.Fake.Authentication.JwtBearer.Tests.Tools;
using Xunit;

namespace WebMotions.Fake.Authentication.JwtBearer.Tests
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
            scheme.Should().NotBeNull();
            scheme.HandlerType.Name.Should().Be("FakeJwtBearerHandler");
            scheme.DisplayName.Should().BeNull();
        }

        [Fact]
        public async Task CustomHeaderReceived()
        {
            var server = CreateServer(o =>
            {
                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var claims = new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, "Bob le Magnifique"),
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
            response.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ResponseText.Should().Be("Bob le Magnifique");
        }

        [Fact]
        public async Task SetRoleReceived()
        {
            var server = CreateServer(o =>
            {
                o.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        Assert.Contains(context.Principal.Claims, a => a.Type == ClaimTypes.Role && a.Value == "Role1");
                        Assert.Contains(context.Principal.Claims, a => a.Type == ClaimTypes.Role && a.Value == "Role2");
                        return Task.FromResult<object>(null);
                    }
                };
            });

            var response = await SendAsync(server.CreateClient().SetFakeBearerToken("SUperUserName", new[] { "Role1", "Role2" }), "http://example.com/oauth");
            response.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ResponseText.Should().Be("SUperUserName");
        }

        [Fact]
        public async Task SetCustomsClaim()
        {
            var server = CreateServer(o =>
            {
                o.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        Assert.Contains(context.Principal.Claims, a => a.Type == ClaimTypes.Role && a.Value == "Role1");
                        Assert.Contains(context.Principal.Claims, a => a.Type == ClaimTypes.Role && a.Value == "Role2");
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
            response.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ResponseText.Should().Be("SUperUserName");
        }

        [Fact]
        public async Task SettingCustomToken()
        {
            var server = CreateServer(o =>
            {
                o.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        Assert.Contains(context.Principal.Claims, a => a.Type == "name" && a.Value == "Kathy Daugherty");
                        Assert.True(context.Principal.Claims.Where(c => c.Type == "name").ToList().Count == 1);
                        Assert.Contains(context.Principal.Claims, a => a.Type == ClaimTypes.NameIdentifier && a.Value == "c611495c-ceb7-0af5-5014-1ecbe067363c");
                        Assert.True(context.Principal.Claims.Where(c => c.Type == ClaimTypes.Role).ToList().Count == 2);
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
            response.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ResponseText.Should().Be("c611495c-ceb7-0af5-5014-1ecbe067363c");
        }

        [Fact]
        public async Task SettingCustomTokenOverridingSubject()
        {
            var server = CreateServer(o =>
            {
                o.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        Assert.Contains(context.Principal.Claims, a => a.Type == "name" && a.Value == "Earl Becker");
                        Assert.DoesNotContain(context.Principal.Claims, a => a.Type == ClaimTypes.NameIdentifier && a.Value == "c611495c-ceb7-0af5-5014-1ecbe067363c");
                        Assert.Contains(context.Principal.Claims, a => a.Type == ClaimTypes.NameIdentifier && a.Value == "Earl Becker");
                        Assert.True(context.Principal.Claims.Where(c => c.Type == "name").ToList().Count == 1);
                        Assert.True(context.Principal.Claims.Where(c => c.Type == ClaimTypes.Role).ToList().Count == 2);
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
            response.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ResponseText.Should().Be("Earl Becker");
        }

        [Fact]
        public async Task SetClaimIssuer()
        {
            var server = CreateServer(o =>
            {
                o.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        Assert.Contains(context.Principal.Claims, a => a.Issuer == "my_custom_issuer_from_jwt");
                        Assert.Contains(context.Principal.Claims, a => a.OriginalIssuer == "my_custom_issuer_from_jwt");
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
                iss = "my_custom_issuer_from_jwt",
                role = new[]
                {
                    "admins",
                    "users"
                }
            });

            var response = await SendAsync(client, "http://example.com/oauth");
            response.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ResponseText.Should().Be("c611495c-ceb7-0af5-5014-1ecbe067363c");
        }

        [Fact]
        public async Task SetClaimIssuerOverride()
        {
            var server = CreateServer(o =>
            {
                ((FakeJwtBearerClaimsHandler)o.SecurityTokenClaimHandler).Options.OverrideIssuer = true;
                ((FakeJwtBearerClaimsHandler)o.SecurityTokenClaimHandler).Options.Issuer = "my_custom_issuer";
                ((FakeJwtBearerClaimsHandler)o.SecurityTokenClaimHandler).Options.OriginalIssuer = "my_original_issuer";
                o.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        Assert.Contains(context.Principal.Claims, a => a.Issuer == "my_custom_issuer");
                        Assert.Contains(context.Principal.Claims, a => a.OriginalIssuer == "my_original_issuer");
                        return Task.FromResult<object>(null);
                    }
                };
            });

            var response = await SendAsync(server.CreateClient().SetFakeBearerToken("SuperUserName", new[] { "Role1" }), "http://example.com/oauth");
            response.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ResponseText.Should().Be("SuperUserName");
        }

        [Fact]
        public async Task SetNameAndRoleRoleClaimOverride()
        {
            var server = CreateServer(
                o =>
                {
                    ((FakeJwtBearerClaimsHandler)o.SecurityTokenClaimHandler).Options.NameClaimType = "custom_name_claim";
                    ((FakeJwtBearerClaimsHandler)o.SecurityTokenClaimHandler).Options.RoleClaimType = "custom_role_claim";

                    o.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            context.Principal.Identity.Name.Should().Be("Kathy Daugherty");
                            context.Principal.IsInRole("admins").Should().BeTrue();
                            return Task.FromResult<object>(null);
                        }
                    };
                },
                claimType: "custom_name_claim");

            var client = server.CreateClient().SetFakeBearerToken(new
            {
                custom_name_claim = "Kathy Daugherty",
                custom_role_claim = new[]
                {
                    "admins",
                    "users"
                }
            });

            var response = await SendAsync(client, "http://example.com/oauth");
            response.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ResponseText.Should().Be("Kathy Daugherty");
        }

        [Theory]
        [InlineData(ClaimTypes.NameIdentifier, true)]
        [InlineData("sub", false)]
        public async Task SetClaimIssuerOverrideUsingJwt(string claimType, bool mapInboundClaims)
        {
            var server = CreateServer(
                o =>
                {
                    o.BearerValueType = FakeJwtBearerBearerValueType.Jwt;
                    ((FakeJwtBearerClaimsHandler)o.SecurityTokenClaimHandler).Options.MapInboundClaims = mapInboundClaims;
                    ((FakeJwtBearerClaimsHandler)o.SecurityTokenClaimHandler).Options.OverrideIssuer = true;
                    ((FakeJwtBearerClaimsHandler)o.SecurityTokenClaimHandler).Options.Issuer = "my_custom_issuer";
                    ((FakeJwtBearerClaimsHandler)o.SecurityTokenClaimHandler).Options.OriginalIssuer = "my_original_issuer";
                    o.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            Assert.Contains(context.Principal.Claims, a => a.Issuer == "my_custom_issuer");
                            Assert.Contains(context.Principal.Claims, a => a.OriginalIssuer == "my_original_issuer");
                            return Task.FromResult<object>(null);
                        }
                    };
                },
                claimType: claimType);

            var claims = new Dictionary<string, object>
            {
                { "sub", "SuperUserName" },
                { "unique_name", "SuperUserName" },
                { "roles", new[] { "Role1" } }
            };

            var response = await SendAsync(server.CreateClient().SetFakeJwtBearerToken(claims), "http://example.com/oauth");
            response.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ResponseText.Should().Be("SuperUserName");
        }

        [Fact]
        public async Task CanSendClaimsViaJwt()
        {
            var claimType = "client_id";
            var client = CreateServer(
                options =>
                {
                    options.BearerValueType = FakeJwtBearerBearerValueType.Jwt;
                },
                claimType: claimType).CreateClient();

            var claims = new Dictionary<string, object> { { claimType, "TestClientId" } };
            client.SetFakeJwtBearerToken(claims);

            var response = await SendAsync(client, "http://example.com/oauth");
            response.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ResponseText.Should().Be("TestClientId");
        }

        private static TestServer CreateServer(Action<FakeJwtBearerOptions> options = null, Func<HttpContext, Func<Task>, Task> handlerBeforeAuth = null, string claimType = ClaimTypes.NameIdentifier)
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

                            var identifier = context.User.FindFirst(claimType);
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
                            await context.AuthenticateAsync(FakeJwtBearerDefaults.AuthenticationScheme);
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
