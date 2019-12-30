using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace WebMotions.Fake.Authentication.JwtBearer.Events
{
    /// <summary>
    /// JwtBearerChallengeContext
    /// </summary>
    public class JwtBearerChallengeContext : PropertiesContext<FakeJwtBearerOptions>
    {
        /// <summary>
        /// JwtBearerChallengeContext
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scheme"></param>
        /// <param name="options"></param>
        /// <param name="properties"></param>
        public JwtBearerChallengeContext(
            HttpContext context,
            AuthenticationScheme scheme,
            FakeJwtBearerOptions options,
            AuthenticationProperties properties)
            : base(context, scheme, options, properties) { }

        /// <summary>
        /// Any failures encountered during the authentication process.
        /// </summary>
        public Exception AuthenticateFailure { get; set; }

        /// <summary>
        /// Gets or sets the "error" value returned to the caller as part
        /// of the WWW-Authenticate header. This property may be null when
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets the "error_description" value returned to the caller as part
        /// of the WWW-Authenticate header. This property may be null when
        /// </summary>
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Gets or sets the "error_uri" value returned to the caller as part of the
        /// WWW-Authenticate header. This property is always null unless explicitly set.
        /// </summary>
        public string ErrorUri { get; set; }

        /// <summary>
        /// If true, will skip any default logic for this challenge.
        /// </summary>
        public bool Handled { get; private set; }

        /// <summary>
        /// Skips any default logic for this challenge.
        /// </summary>
        public void HandleResponse() => Handled = true;
    }
}