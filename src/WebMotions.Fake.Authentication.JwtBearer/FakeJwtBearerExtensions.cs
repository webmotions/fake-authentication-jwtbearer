using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace WebMotions.Fake.Authentication.JwtBearer
{
    /// <summary>
    /// FakeJwtBearerExtensions
    /// </summary>
    public static class FakeJwtBearerExtensions
    {
        /// <summary>
        /// AuthenticationBuilder
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddFakeJwtBearer(this AuthenticationBuilder builder)
                => builder.AddFakeJwtBearer(FakeJwtBearerDefaults.AuthenticationScheme, _ => { });

        /// <summary>
        /// AuthenticationBuilder
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddFakeJwtBearer(this AuthenticationBuilder builder, Action<FakeJwtBearerOptions> configureOptions)
            => builder.AddFakeJwtBearer(FakeJwtBearerDefaults.AuthenticationScheme, configureOptions);

        /// <summary>
        /// AuthenticationBuilder
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="authenticationScheme"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddFakeJwtBearer(this AuthenticationBuilder builder, string authenticationScheme, Action<FakeJwtBearerOptions> configureOptions)
            => builder.AddFakeJwtBearer(authenticationScheme, displayName: null, configureOptions: configureOptions);

        /// <summary>
        /// AuthenticationBuilder
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="authenticationScheme"></param>
        /// <param name="displayName"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddFakeJwtBearer(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<FakeJwtBearerOptions> configureOptions)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<FakeJwtBearerOptions>, FakeJwtBearerPostConfigureOptions>());
            return builder.AddScheme<FakeJwtBearerOptions, FakeJwtBearerHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}