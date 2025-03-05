using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebMotions.Fake.Authentication.JwtBearer
{
    /// <summary>
    /// Options class provides information needed to control Claims Handler behavior
    /// </summary>
    public class FakeJwtBearerClaimsHandlerOptions
    {
        /// <summary>
        /// Gets or sets whether to override the iss claim (if found) in the JWT with the <see cref="Issuer"/> and <see cref="OriginalIssuer"/>.
        /// </summary>
        public bool OverrideIssuer { get; set; }

        /// <summary>
        /// Gets or sets the claim identity issuer. Defaults to <see cref="ClaimsIdentity.DefaultIssuer"/>.
        /// </summary>
        public string Issuer { get; set; } = ClaimsIdentity.DefaultIssuer;

        /// <summary>
        /// Gets or sets the claim identity original issuer. Defaults to <see cref="ClaimsIdentity.DefaultIssuer"/>.
        /// </summary>
        public string OriginalIssuer { get; set; } = ClaimsIdentity.DefaultIssuer;

        /// <summary>
        /// Gets or sets whether to map the inbound claims using the inbound claims found in <see cref="JwtSecurityTokenHandler.DefaultInboundClaimTypeMap"/>. Defaults to <value>true</value>
        /// </summary>
        public bool MapInboundClaims { get; set; } = true;

        /// <summary>
        /// The claim type to use for name claims. Defaults to <see cref="ClaimTypes.Name"/>.
        /// </summary>
        public string NameClaimType { get; set; } = ClaimTypes.Name;

        /// <summary>
        /// The claim type to use for role claims. Defaults to <see cref="ClaimTypes.Role"/>.
        /// </summary>
        public string RoleClaimType { get; set; } = ClaimTypes.Role;
    }
}