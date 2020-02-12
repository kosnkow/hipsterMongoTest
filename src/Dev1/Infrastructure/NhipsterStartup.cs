using JHipsterNet.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MyCompany.Infrastructure {
    public static class NhipsterSettingsConfiguration {
        public static IServiceCollection AddNhipsterModule(this IServiceCollection @this, IConfiguration configuration)
        {
//            https://andrewlock.net/how-to-use-the-ioptions-pattern-for-configuration-in-asp-net-core-rc2/
//            https://andrewlock.net/adding-validation-to-strongly-typed-configuration-objects-in-asp-net-core/
            @this.Configure<JHipsterSettings>(configuration.GetSection("jhipster"));
            @this.AddScoped<JHipsterSettings, JHipsterSettings>();
//it doesn't work
//            @this.AddSingleton(resolver => resolver.GetRequiredService<IOptions<NhipsterSettings>>().Value);
            return @this;
        }
    }
}
