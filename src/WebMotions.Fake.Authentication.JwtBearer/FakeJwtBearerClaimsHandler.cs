using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace WebMotions.Fake.Authentication.JwtBearer
{
    /// <inheritdoc />
    public class FakeJwtBearerClaimsHandler : ISecurityTokenClaimsHandler
    {
        private readonly JwtSecurityTokenHandler _securityTokenHandler = new();

        /// <summary>
        /// Gets the options to configure the handler
        /// </summary>
        public FakeJwtBearerClaimsHandlerOptions Options { get; }

        /// <summary>
        /// Construct a FakeJwtBearerClaimsHandler
        /// </summary>
        /// <param name="options">The claims handler options</param>
        public FakeJwtBearerClaimsHandler(FakeJwtBearerClaimsHandlerOptions options)
        {
            Options = options;
        }

        /// <inheritdoc />
        public virtual ClaimsIdentity CreateClaimsIdentity(Dictionary<string, JsonElement> token)
        {
            var identity = new ClaimsIdentity("FakeJwtBearer");

            string issuer;
            string originalIssuer;
            if (!Options.OverrideIssuer)
            {
                if (token.TryGetValue("iss", out var issJWTValue))
                {
                    if (issJWTValue.ValueKind != JsonValueKind.String)
                        throw new Exception("The iss claim is not a string");

                    issuer = originalIssuer = issJWTValue.GetString();
                }
                else
                {
                    issuer = Options.Issuer;
                    originalIssuer = Options.OriginalIssuer;
                }
            }
            else
            {
                issuer = Options.Issuer;
                originalIssuer = Options.OriginalIssuer;
            }

            foreach (var prop in token)
            {
                Claim claim;
                if (prop.Value.ValueKind == JsonValueKind.String)
                {
                    claim = CreateClaim(prop.Key, prop.Value.GetString(), identity, issuer, originalIssuer);
                    if (claim != null)
                    {
                        identity.AddClaim(claim);
                    }
                }
                else if (prop.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var subValue in prop.Value.EnumerateArray())
                    {
                        claim = CreateClaim(prop.Key, subValue.GetString(), identity, issuer, originalIssuer);
                        if (claim != null)
                        {
                            identity.AddClaim(claim);
                        }
                    }
                }
                else
                {
                    throw new Exception("Unknown type");
                }
            }

            return identity;
        }

        /// <summary>
        /// Creates a claims identity from a JWT token
        /// </summary>
        /// <param name="jwtToken">The JWT token</param>
        /// <returns>A <see cref="ClaimsIdentity"/></returns>
        public virtual ClaimsIdentity CreateClaimsIdentity(string jwtToken)
        {
            var identity = new ClaimsIdentity("FakeJwtBearer");
            var tokenHandler = new JwtSecurityTokenHandler();
            if (tokenHandler.ReadToken(jwtToken) is not JwtSecurityToken securityToken)
            {
                return identity;
            }

            foreach (var claim in securityToken.Claims)
            {
                identity.AddClaim(claim);
            }

            return identity;
        }

        private Claim CreateClaim(string key, string value, ClaimsIdentity identity, string issuer, string originalIssuer)
        {
            if (_securityTokenHandler.InboundClaimFilter.Contains(key))
                return null;

            bool wasMapped = true;
            if (!_securityTokenHandler.InboundClaimTypeMap.TryGetValue(key, out var claimType))
            {
                claimType = key;
                wasMapped = false;
            }

            if (claimType == ClaimTypes.Actor)
            {
                throw new Exception($"{ClaimTypes.Actor} is not supported");
            }

            Claim claim = new Claim(claimType, value, value.GetType().ToString(), issuer, originalIssuer, identity);

            if (wasMapped)
                claim.Properties[JwtSecurityTokenHandler.ShortClaimTypeProperty] = key;

            return claim;
        }
    }
}