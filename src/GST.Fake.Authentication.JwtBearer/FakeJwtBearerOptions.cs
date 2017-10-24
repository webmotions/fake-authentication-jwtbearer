using Microsoft.AspNetCore.Authentication;
using GST.Fake.Authentication.JwtBearer;

namespace GST.Fake.Builder
{
    /// <summary>
    /// Options class provides information needed to control Bearer Authentication middleware behavior
    /// </summary>
    public class FakeJwtBearerOptions : AuthenticationSchemeOptions
    {

        /// <summary>
        /// Gets or sets the challenge to put in the "WWW-Authenticate" header.
        /// </summary>
        public string Challenge { get; set; } = FakeJwtBearerDefaults.AuthenticationScheme;

        /// <summary>
        /// Defines whether the bearer token should be stored in the
        /// </summary>
        public bool SaveToken { get; set; } = true;
    }
}
