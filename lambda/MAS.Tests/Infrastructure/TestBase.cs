using MAS.Configuration;
using MAS.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http;

namespace MAS.Tests.Infrastructure
{
    public class TestBase
    {
        protected readonly TestServer _server;
        protected readonly HttpClient _client;
        protected readonly IConfigurationRoot _config;

        public TestBase()
        {
            _config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .AddUserSecrets("adafe3d8-65fb-49fd-885e-03341a36dc88")
               .Build();

            var builder = new WebHostBuilder()
                .UseContentRoot("../../../../MAS")
                .ConfigureServices(services =>
                {
                    AppSettings.Configure(services, _config);
                })
                .UseEnvironment("Production")
                .UseStartup(typeof(Startup));
            _server = new TestServer(builder);
            _client = _server.CreateClient(); 
        }
    }
}
