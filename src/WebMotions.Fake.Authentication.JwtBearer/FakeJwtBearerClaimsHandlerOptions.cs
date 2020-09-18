using System.Security.Claims;

namespace WebMotions.Fake.Authentication.JwtBearer
{
    /// <summary>
    /// Options class provides information needed to control Claims Handler behavior
    /// </summary>
    public class FakeJwtBearerClaimsHandlerOptions
    {
        /// <summary>
        /// Gets or sets the claim identity issuer. Defaults to <see cref="ClaimsIdentity.DefaultIssuer"/>
        /// </summary>
        public string Issuer { get; set; } = ClaimsIdentity.DefaultIssuer;

        /// <summary>
        /// Gets or sets the claim identity original issuer. Defaults to <see cref="ClaimsIdentity.DefaultIssuer"/>
        /// </summary>
        public string OriginalIssuer { get; set; } = ClaimsIdentity.DefaultIssuer;
    }
}