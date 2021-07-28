using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace WebMotions.Fake.Authentication.JwtBearer.Events
{
    /// <summary>
    /// ForbiddenContext
    /// </summary>
    public class ForbiddenContext : ResultContext<FakeJwtBearerOptions>
    {
        /// <summary>
        /// ForbiddenContext
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scheme"></param>
        /// <param name="options"></param>
        public ForbiddenContext(
            HttpContext context,
            AuthenticationScheme scheme,
            FakeJwtBearerOptions options)
            : base(context, scheme, options)
        {
        }
    }
}