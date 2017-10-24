using System;
using GST.Fake.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;

namespace GST.Fake.Builder
{
    /// <summary>
    /// Extension methods to add OpenIdConnect Bearer authentication capabilities to an HTTP application pipeline.
    /// </summary>
    public static class FakeJwtBearerAppBuilderExtensions
    {
        /// <summary>
        /// Adds the JwtBearerMiddleware middleware to the specified IApplicationBuilder, which enables Bearer token processing capabilities.
        /// This middleware understands appropriately
        /// formatted and secured tokens which appear in the request header. If the Options.AuthenticationMode is Active, the
        /// claims within the bearer token are added to the current request's IPrincipal User. If the Options.AuthenticationMode 
        /// is Passive, then the current request is not modified, but IAuthenticationManager AuthenticateAsync may be used at
        /// any time to obtain the claims from the request's bearer token.
        /// See also http://tools.ietf.org/html/rfc6749
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470", error: true)]
        public static IApplicationBuilder UseFakeJwtBearerAuthentication(this IApplicationBuilder app)
        {
            throw new NotSupportedException("This method is no longer supported, see https://go.microsoft.com/fwlink/?linkid=845470");
        }

        /// <summary>
        /// Adds the JwtBearerMiddleware middleware to the specified IApplicationBuilder, which enables Bearer token processing capabilities.
        /// This middleware understands appropriately
        /// formatted and secured tokens which appear in the request header. If the Options.AuthenticationMode is Active, the
        /// claims within the bearer token are added to the current request's IPrincipal User. If the Options.AuthenticationMode 
        /// is Passive, then the current request is not modified, but IAuthenticationManager AuthenticateAsync may be used at
        /// any time to obtain the claims from the request's bearer token.
        /// See also http://tools.ietf.org/html/rfc6749
        /// </summary>
        /// <param name="app">The IApplicationBuilder to add the middleware to.</param>
        /// <param name="options">A JwtBearerOptions that specifies options for the middleware.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        [Obsolete("See https://go.microsoft.com/fwlink/?linkid=845470", error: true)]
        public static IApplicationBuilder UseFakeJwtBearerAuthentication(this IApplicationBuilder app, FakeJwtBearerOptions options)
        {
            throw new NotSupportedException("This method is no longer supported, see https://go.microsoft.com/fwlink/?linkid=845470");
        }
    }
}
