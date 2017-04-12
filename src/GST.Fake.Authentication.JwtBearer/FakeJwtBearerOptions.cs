using Microsoft.AspNetCore.Builder;
using GST.Fake.Authentication.JwtBearer;

namespace GST.Fake.Builder
{
    /// <summary>
    /// Options class provides information needed to control Bearer Authentication middleware behavior
    /// </summary>
    public class FakeJwtBearerOptions : AuthenticationOptions
    {
        /// <summary>
        /// Creates an instance of bearer authentication options with default values.
        /// </summary>
        public FakeJwtBearerOptions() : base()
        {
            AuthenticationScheme = FakeJwtBearerDefaults.AuthenticationScheme;
            AutomaticAuthenticate = true;
            AutomaticChallenge = true;
        }

        /// <summary>
        /// Gets or sets the challenge to put in the "WWW-Authenticate" header.
        /// </summary>
        public string Challenge { get; set; } = FakeJwtBearerDefaults.AuthenticationScheme;
    }
}
