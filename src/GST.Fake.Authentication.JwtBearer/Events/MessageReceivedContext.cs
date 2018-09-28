using GST.Fake.Builder;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace GST.Fake.Authentication.JwtBearer.Events
{
    /// <summary>
    /// Context who hold the token
    /// </summary>
    public class MessageReceivedContext : ResultContext<FakeJwtBearerOptions>
    {
        /// <summary>
        /// MessageReceivedContext
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scheme"></param>
        /// <param name="options"></param>
        public MessageReceivedContext(
           HttpContext context,
           AuthenticationScheme scheme,
           FakeJwtBearerOptions options)
           : base(context, scheme, options) { }

        /// <summary>
        /// Bearer Token. This will give the application an opportunity to retrieve a token from an alternative location.
        /// </summary>
        public string Token { get; set; }
    }
}
