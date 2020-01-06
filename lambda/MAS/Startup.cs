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
        public readonly static RegionEndpoint Region = RegionEndpoint.EUWest1;
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

            AppSettings.Configure(services, Configuration);
            
            services.TryAddSingleton<ISeriLogger, SeriLogger>();
            services.TryAddSingleton<IStaticContentService, S3Service>();
            services.TryAddSingleton<IViewRenderer, ViewRenderer>();
            services.TryAddTransient<IMailService, MailService>();
            services.TryAddTransient<IContentService, ContentService>();

            services.AddMailChimpClient(AppSettings.MailConfig.ApiKey);

            AmazonS3Config s3config;
            if (AppSettings.EnvironmentConfig.Name == "local")  //TODO: Should use Environment.IsDevelopment() here. When running tests it returns "Production"
            {
                s3config = new AmazonS3Config()
                {
                    RegionEndpoint = Region,
                    ServiceURL = AppSettings.AWSConfig.ServiceURL,
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
            services.AddTransient<IAmazonS3>((sP) => {
                return new AmazonS3Client(AppSettings.AWSConfig.AccessKey, AppSettings.AWSConfig.SecretKey, s3config);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ISeriLogger seriLogger, IApplicationLifetime appLifetime )
        {
            seriLogger.Configure(loggerFactory, Configuration, appLifetime, env);
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
