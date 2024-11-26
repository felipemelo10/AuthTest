using AuthAspNet.Services;

namespace AuthTest.Configuration;

internal static class DependencyInjectionConfig
{
    internal static WebApplicationBuilder AddDependencyInjection(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<AuthService>();

        return builder;
    }
}
