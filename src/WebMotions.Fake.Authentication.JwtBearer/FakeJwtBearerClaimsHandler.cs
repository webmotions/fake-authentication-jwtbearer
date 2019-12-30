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
        private readonly JwtSecurityTokenHandler _securityTokenHandler = new JwtSecurityTokenHandler();

        /// <inheritdoc />
        public virtual ClaimsIdentity CreateClaimsIdentity(Dictionary<string, JsonElement> token)
        {
            var identity = new ClaimsIdentity("FakeJwtBearer");

            foreach (var prop in token)
            {
                Claim claim;
                if (prop.Value.ValueKind == JsonValueKind.String)
                {
                    claim = CreateClaim(prop.Key, prop.Value.GetString(), identity);
                    if (claim != null)
                    {
                        identity.AddClaim(claim);
                    }
                }
                else if (prop.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var subValue in prop.Value.EnumerateArray())
                    {
                        claim = CreateClaim(prop.Key, subValue.GetString(), identity);
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

        private Claim CreateClaim(string key, string value, ClaimsIdentity identity)
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

            Claim claim = new Claim(claimType, value, value.GetType().ToString(), ClaimsIdentity.DefaultIssuer, ClaimsIdentity.DefaultIssuer, identity);

            if (wasMapped)
                claim.Properties[JwtSecurityTokenHandler.ShortClaimTypeProperty] = key;

            return claim;
        }
    }
}