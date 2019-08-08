using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GST.Fake.Authentication.JwtBearer
{
    /// <inheritdoc />
    public class FakeJwtBearerClaimsHandler : ISecurityTokenClaimsHandler
    {
        private readonly JwtSecurityTokenHandler _securityTokenHandler = new JwtSecurityTokenHandler();

        /// <inheritdoc />
        public virtual ClaimsIdentity CreateClaimsIdentity(Dictionary<string, dynamic> token)
        {
            var identity = new ClaimsIdentity("FakeJwtBearer");

            foreach (var prop in token)
            {
                Claim claim;
                if (prop.Value is string)
                {
                    claim = CreateClaim(prop.Key, prop.Value, identity);
                    if (claim != null)
                    {
                        identity.AddClaim(claim);
                    }
                }
                else if (prop.Value is IEnumerable)
                {
                    foreach (string subValue in prop.Value)
                    {
                        claim = CreateClaim(prop.Key, subValue, identity);
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