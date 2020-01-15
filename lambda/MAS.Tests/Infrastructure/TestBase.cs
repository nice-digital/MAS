using MAS.Configuration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace MAS.Tests.Infrastructure
{
    public class TestBase : IDisposable
    {
        protected readonly IWebHostBuilder _builder;
        protected readonly TestServer _server;
        protected readonly HttpClient _client;
        //protected readonly IConfigurationRoot _config;

        public TestBase()
        {
            //_config = new ConfigurationBuilder()
            //   .AddJsonFile("appsettings.json")
            //   //.AddUserSecrets("adafe3d8-65fb-49fd-885e-03341a36dc88")
            //   .Build();

            //var builder = new WebHostBuilder()
            _builder = WebHost.CreateDefaultBuilder(new string[0])
                .UseContentRoot("../../../../MAS")
                .ConfigureServices(services =>
                {
                    //AppSettings.Configure(services, _config);
                })
                .UseEnvironment("Production")
                .UseStartup(typeof(Startup));

            _server = new TestServer(_builder);
            _client = _server.CreateClient();
            //_client.BaseAddress = new Uri("http://localhost:5000");
        }

        protected void WithImplementation<TService, TImplementation>()
            where TService : class where TImplementation : class, TService
        {
            _builder.ConfigureServices(serviceCollection => serviceCollection.AddTransient<TService, TImplementation>());
        }

        protected void WithImplementation<TService>(TService implementation)
            where TService : class
        {
            _builder.ConfigureServices(serviceCollection => serviceCollection.AddTransient(serviceProvier => implementation));
        }

        public virtual void Dispose()
        {
            // Do "global" teardown here; Called after every test method.
            _client.Dispose();
            _server.Dispose();
        }
    }
}
