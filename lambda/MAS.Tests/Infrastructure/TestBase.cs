using MAS.Configuration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MAS.Tests.Infrastructure
{
    public abstract class TestBase : IDisposable
    {
        protected readonly MASWebApplicationFactory _factory;

        public TestBase()
        {
            _factory = new MASWebApplicationFactory();
        }

        protected WebApplicationFactory<Startup> WithImplementation<TService>(TService implementation)
        {
            return _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var serviceProvider = services.BuildServiceProvider();

                    var descriptor =
                        new ServiceDescriptor(
                            typeof(TService), implementation);

                    services.Replace(descriptor);
                });
            });
        }

        public virtual void Dispose()
        {
            _factory.Dispose();
        }
    }
}
