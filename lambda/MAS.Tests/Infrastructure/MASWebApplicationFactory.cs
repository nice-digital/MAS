using MAS.Configuration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MAS.Tests.Infrastructure
{
    /// <summary>
    /// See https://github.com/aspnet/AspNetCore.Docs/issues/7063#issuecomment-414661566
    /// </summary>
    public class MASWebApplicationFactory : WebApplicationFactory<Startup>
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .UseLambdaServer();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var projectDir = Directory.GetCurrentDirectory();

            builder.ConfigureAppConfiguration((context, conf) =>
            {
                conf.AddJsonFile(Path.Combine(projectDir, "appsettings.test.json"));
            });

            builder.ConfigureTestServices(services => {
                AppSettings.CMSConfig.BaseUrl = new Uri("file://" + projectDir + "/Feeds").ToString();
            });

            base.ConfigureWebHost(builder);
        }
    }
}
