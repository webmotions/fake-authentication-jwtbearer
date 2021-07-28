using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace WebMotions.Fake.Authentication.JwtBearer.Events
{
    /// <summary>
    /// AuthenticationFailedContext
    /// </summary>
    public class AuthenticationFailedContext : ResultContext<FakeJwtBearerOptions>
    {
        /// <summary>
        /// AuthenticationFailedContext
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scheme"></param>
        /// <param name="options"></param>
        public AuthenticationFailedContext(
            HttpContext context,
            AuthenticationScheme scheme,
            FakeJwtBearerOptions options)
            : base(context, scheme, options)
        {
        }

        /// <summary>
        /// Exception
        /// </summary>
        public Exception Exception { get; set; }
    }
}
