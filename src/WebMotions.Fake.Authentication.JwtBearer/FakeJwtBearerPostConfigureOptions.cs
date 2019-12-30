using Microsoft.Extensions.Options;

namespace WebMotions.Fake.Authentication.JwtBearer
{
    /// <summary>
    /// Used to setup defaults for all <see cref="FakeJwtBearerOptions"/>.
    /// </summary>
    public class FakeJwtBearerPostConfigureOptions : IPostConfigureOptions<FakeJwtBearerOptions>
    {
        /// <summary>
        /// Invoked to post configure a JwtBearerOptions instance.
        /// </summary>
        /// <param name="name">The name of the options instance being configured.</param>
        /// <param name="options">The options instance to configure.</param>
        public void PostConfigure(string name, FakeJwtBearerOptions options)
        {

        }
    }
}
