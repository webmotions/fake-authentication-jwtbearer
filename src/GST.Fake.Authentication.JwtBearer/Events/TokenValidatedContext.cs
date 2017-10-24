using GST.Fake.Builder;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace GST.Fake.Authentication.JwtBearer.Events
{
    public class TokenValidatedContext : ResultContext<FakeJwtBearerOptions>
    {
        public TokenValidatedContext(
            HttpContext context,
            AuthenticationScheme scheme,
            FakeJwtBearerOptions options)
            : base(context, scheme, options) { }

        //public SecurityToken SecurityToken { get; set; }
    }
}