using GST.Fake.Builder;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;

namespace GST.Fake.Authentication.JwtBearer.Events
{
    public class AuthenticationFailedContext : ResultContext<FakeJwtBearerOptions>
    {
        public AuthenticationFailedContext(
            HttpContext context,
            AuthenticationScheme scheme,
            FakeJwtBearerOptions options)
            : base(context, scheme, options) { }

        public Exception Exception { get; set; }
    }
}
