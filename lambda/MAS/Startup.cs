using Amazon;
using Amazon.CloudFront;
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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace MAS
{
    public class Startup
    {
        public static readonly RegionEndpoint Region = RegionEndpoint.EUWest1;
        public static IConfiguration Configuration { get; private set; }
        public IHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddLogging(builder => {
                builder.AddILoggingBuilderInstance();
            });

            services.TryAddSingleton<ISeriLogger, SeriLogger>();
            services.TryAddSingleton<IStaticWebsiteService, S3StaticWebsiteService>();
            services.TryAddSingleton<IViewRenderer, ViewRenderer>();
            services.TryAddTransient<IMailService, MailService>();
            services.TryAddTransient<IContentService, ContentService>();
            services.TryAddTransient<IBankHolidayService, BankHolidayService>();

            EnvironmentConfig environmentConfig = new EnvironmentConfig();
            AWSConfig awsConfig = new AWSConfig();
            CloudFrontConfig cloudFrontConfig = new CloudFrontConfig();
            CMSConfig cmsConfig = new CMSConfig();
            MailChimpConfig mailChimpConfig = new MailChimpConfig();
            MailConfig mailConfig = new MailConfig();
            BankHolidayConfig bankHolidayConfig = new BankHolidayConfig();

            Configuration.Bind("AppSettings:Environment", environmentConfig);
            Configuration.Bind("AWS", awsConfig);
            Configuration.Bind("AWS:CloudFront", cloudFrontConfig);
            Configuration.Bind("CMS", cmsConfig);
            Configuration.Bind("MailChimp", mailChimpConfig);
            Configuration.Bind("Mail", mailConfig);
            Configuration.Bind("BankHoliday", bankHolidayConfig);

            services.AddSingleton(environmentConfig)
            .AddSingleton(awsConfig)
            .AddSingleton(cloudFrontConfig)
            .AddSingleton(cmsConfig)
            .AddSingleton(mailChimpConfig)
            .AddSingleton(mailConfig)
            .AddSingleton(bankHolidayConfig);

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

            var cloudfrontConfig = new AmazonCloudFrontConfig(){ RegionEndpoint = Region };
            services.AddTransient<IAmazonCloudFront>((acf) =>
            {
                return new AmazonCloudFrontClient(awsConfig.AccessKey, awsConfig.SecretKey, cloudfrontConfig);
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app,
            IHostEnvironment env,
            ILoggingBuilder loggerFactory,
            ISeriLogger seriLogger,
            IHostApplicationLifetime appLifetime,
            EnvironmentConfig environmentConfig)
        {
            seriLogger.Configure(loggerFactory, Configuration, appLifetime, env, environmentConfig);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                loggerFactory.AddConfiguration(Configuration.GetSection("Logging"));
                loggerFactory.AddConsole();
                loggerFactory.AddDebug();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            
            app.UseRouting();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });


        }
    }
    public static class LoggingBuilderExtensions
    {
        public static ILoggingBuilder AddILoggingBuilderInstance(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton(builder);
            return builder;
        }
    }
}
