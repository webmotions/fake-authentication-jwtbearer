using System.Collections.Generic;
using System.Security.Claims;

namespace GST.Fake.Authentication.JwtBearer
{
    /// <summary>
    /// Contains the logic to handler the creation of the claims identity
    /// </summary>
    public interface ISecurityTokenClaimsHandler
    {
        /// <summary>
        /// Creates a claims identity
        /// </summary>
        /// <param name="token">The token data</param>
        /// <returns>A <see cref="ClaimsIdentity"/></returns>
        ClaimsIdentity CreateClaimsIdentity(Dictionary<string, dynamic> token);
    }
}