using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MAS.Configuration
{
    public static class AppSettings
    {
        // this is a static class for storing appsettings so we don't have to use DI for passing configuration stuff
        // (i.e. to stop us having to pass IOptions<SomeConfig> through the stack)
        public static EnvironmentConfig EnvironmentConfig { get; private set; }
        public static AWSConfig AWSConfig { get; set; }
        public static CMSConfig CMSConfig { get; set; }
        
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EnvironmentConfig>(configuration.GetSection("AppSettings:Environment"));
            services.Configure<AWSConfig>(configuration.GetSection("AWS"));
            services.Configure<CMSConfig>(configuration.GetSection("CMS"));
          
            var sp = services.BuildServiceProvider();
            EnvironmentConfig = sp.GetService<IOptions<EnvironmentConfig>>().Value;
            AWSConfig = sp.GetService<IOptions<AWSConfig>>().Value;
            CMSConfig = sp.GetService<IOptions<CMSConfig>>().Value;
            
        }
    }
}
