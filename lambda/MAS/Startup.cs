using Amazon;
using Amazon.S3;
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
        public const string AppS3BucketKey = "AppS3Bucket";
        public readonly static RegionEndpoint Region = RegionEndpoint.EUWest1;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            AppSettings.Configure(services, Configuration);
            
            services.TryAddSingleton<ISeriLogger, SeriLogger>();
            services.TryAddSingleton<IS3Service, S3Service>();

            services.AddTransient<IAmazonS3>((sP) => {
                var s3config = new AmazonS3Config()
                {
                    RegionEndpoint = Region,
                    ServiceURL = AppSettings.AWSConfig.ServiceURL,
                    ForcePathStyle = true
                };

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
