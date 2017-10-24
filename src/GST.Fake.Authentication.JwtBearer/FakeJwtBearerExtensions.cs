using System;
using Microsoft.AspNetCore.Authentication;
using GST.Fake.Builder;
using GST.Fake.Authentication.JwtBearer;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FakeJwtBearerExtensions
    {
        public static AuthenticationBuilder AddFakeJwtBearer(this AuthenticationBuilder builder)
                => builder.AddFakeJwtBearer(FakeJwtBearerDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddFakeJwtBearer(this AuthenticationBuilder builder, Action<FakeJwtBearerOptions> configureOptions)
            => builder.AddFakeJwtBearer(FakeJwtBearerDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddFakeJwtBearer(this AuthenticationBuilder builder, string authenticationScheme, Action<FakeJwtBearerOptions> configureOptions)
            => builder.AddFakeJwtBearer(authenticationScheme, displayName: null, configureOptions: configureOptions);

        public static AuthenticationBuilder AddFakeJwtBearer(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<FakeJwtBearerOptions> configureOptions)
        {
            //builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<FakeJwtBearerOptions>, FakeJwtBearerPostConfigureOptions>());
            return builder.AddScheme<FakeJwtBearerOptions, FakeJwtBearerHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}