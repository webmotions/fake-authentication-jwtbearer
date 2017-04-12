using System;
using System.Security.Claims;
using System.Threading.Tasks;
using GST.Fake.Builder;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections;

namespace GST.Fake.Authentication.JwtBearer
{
    internal class FakeJwtBearerHandler : AuthenticationHandler<FakeJwtBearerOptions>
    {

        /// <summary>
        /// Searches the 'Authorization' header for a 'FakeBearer' token. If the 'FakeBearer' token is found, it is validated using <see cref="TokenValidationParameters"/> set in the options.
        /// </summary>
        /// <returns></returns>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string token = null;
            try
            {
                string authorization = Request.Headers["Authorization"];

                // If no authorization header found, nothing to process further
                if (string.IsNullOrEmpty(authorization))
                {
                    return Task.FromResult(AuthenticateResult.Skip());
                }

                if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = authorization.Substring("Bearer ".Length).Trim();
                }

                // If no token found, no further work possible
                if (string.IsNullOrEmpty(token))
                {
                    return Task.FromResult(AuthenticateResult.Skip());
                }

                dynamic tokenDecoded = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(token);

                ClaimsIdentity id = new ClaimsIdentity("Identity.Application", "name", "role");

                foreach(var td in tokenDecoded)
                {
                    if(td.Key == "sub")
                    {
                        id.AddClaim(new Claim("sub", td.Value.ToString()));
                        id.AddClaim(new Claim("name", td.Value.ToString()));
                    }
                    else
                    {
                        if (td.Value is string)
                        {
                            id.AddClaim(new Claim(td.Key, td.Value));
                        }
                        else if(td.Value is IEnumerable)
                        {
                            foreach(string subValue in td.Value)
                            { 
                                id.AddClaim(new Claim(td.Key, subValue));
                            }
                        }
                        else
                        {
                            throw new Exception("Unknown type");
                        }
                    }
                }

                ClaimsPrincipal principal = new ClaimsPrincipal(id);

                var ticket = new AuthenticationTicket(principal, new AuthenticationProperties(), Options.AuthenticationScheme);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
        {
            Response.StatusCode = 401;

            return Task.FromResult(false);
        }

        protected override Task HandleSignOutAsync(SignOutContext context)
        {
            throw new NotSupportedException();
        }

        protected override Task HandleSignInAsync(SignInContext context)
        {
            throw new NotSupportedException();
        }
    }
}
