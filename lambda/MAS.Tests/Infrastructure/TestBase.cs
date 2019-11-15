using MAS.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http;

namespace MAS.Tests.Infrastructure
{
    public class TestBase
    {
        protected readonly TestServer _server;
        protected readonly HttpClient _client;

        public TestBase()
        {
            var builder = new WebHostBuilder()
                .UseContentRoot("../../../../MAS")
                .UseEnvironment("Production")
                .UseStartup(typeof(Startup));
            _server = new TestServer(builder);
            _client = _server.CreateClient();
        }
    }
}
