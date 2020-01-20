using Amazon;
using Amazon.S3;
using MailChimp.Net;
using MAS.Configuration;
using MAS.Logging;
using MAS.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace MAS
{
    public class Startup
    {
        public static readonly RegionEndpoint Region = RegionEndpoint.EUWest1;
        public static IConfiguration Configuration { get; private set; }
        public IHostingEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.TryAddSingleton<ISeriLogger, SeriLogger>();
            services.TryAddSingleton<IStaticContentService, S3StaticContentService>();
            services.TryAddSingleton<IViewRenderer, ViewRenderer>();
            services.TryAddTransient<IMailService, MailService>();
            services.TryAddTransient<IContentService, ContentService>();

            EnvironmentConfig environmentConfig = new EnvironmentConfig();
            AWSConfig awsConfig = new AWSConfig();
            CMSConfig cmsConfig = new CMSConfig();
            MailChimpConfig mailChimpConfig = new MailChimpConfig();
            MailConfig mailConfig = new MailConfig();

            Configuration.Bind("AppSettings:Environment", environmentConfig);
            Configuration.Bind("AWS", awsConfig);
            Configuration.Bind("CMS", cmsConfig);
            Configuration.Bind("MailChimp", mailChimpConfig);
            Configuration.Bind("Mail", mailConfig);

            services.AddSingleton(environmentConfig)
                .AddSingleton(awsConfig)
                .AddSingleton(cmsConfig)
                .AddSingleton(mailChimpConfig)
                .AddSingleton(mailConfig);

            services.AddMailChimpClient(mailChimpConfig.ApiKey);

            AmazonS3Config s3config;
            if (environmentConfig.Name == "local")  //TODO: Should use Environment.IsDevelopment() here. When running tests it returns "Production"
            {
                s3config = new AmazonS3Config()
                {
                    RegionEndpoint = Region,
                    ServiceURL = awsConfig.ServiceURL,
                    ForcePathStyle = true
                };
            }
            else
            {
                s3config = new AmazonS3Config()
                {
                    RegionEndpoint = Region
                };
            }
            services.AddTransient<IAmazonS3>((sP) =>
            {
                return new AmazonS3Client(awsConfig.AccessKey, awsConfig.SecretKey, s3config);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            ISeriLogger seriLogger,
            IApplicationLifetime appLifetime,
            EnvironmentConfig environmentConfig)
        {
            seriLogger.Configure(loggerFactory, Configuration, appLifetime, env, environmentConfig);
            loggerFactory.CreateLogger<Startup>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                loggerFactory.AddDebug();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
