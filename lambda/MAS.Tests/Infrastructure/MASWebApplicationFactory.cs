using MailChimp.Net.Interfaces;
using MAS.Configuration;
using MAS.Tests.Extensions;
using MAS.Tests.Fakes;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit.Abstractions;

namespace MAS.Tests.Infrastructure
{
    /// <summary>
    /// See https://github.com/aspnet/AspNetCore.Docs/issues/7063#issuecomment-414661566
    /// </summary>
    public class MASWebApplicationFactory : WebApplicationFactory<Startup>
    {
        #region Constructor

        private readonly IList<Action<IServiceCollection>> serviceCollectionsActions = new List<Action<IServiceCollection>>();
        private readonly ITestOutputHelper _output;
        private Action<CMSConfig> _cmsConfigUpdateAction;

        public MASWebApplicationFactory(ITestOutputHelper output)
        {
            _output = output;
            _output.WriteLine("MASWebApplicationFactory.constructor");
        }

        #endregion

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            _output.WriteLine("MASWebApplicationFactory.CreateWebHostBuilder");

            string projectDir = Directory.GetCurrentDirectory();
            return WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((context, conf) =>
                {
                    conf.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.test.json"));
                })
                .UseLambdaServer();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            _output.WriteLine("MASWebApplicationFactory.ConfigureWebHost");

            builder.ConfigureTestServices(services =>
                {
                    // Default to loading CMS files from the silesystem. Override this with WithCMSConfig if you want
                    services.ReplaceService(TestAppSettings.CMS.Default);
                    services.ReplaceService(TestAppSettings.MailChimp.Default);

                    // We don't want to actually hit real mailchimp in our tests, so fake it
                    services.ReplaceService(new FakeMailChimpManager().Object);

                    foreach (Action<IServiceCollection> action in serviceCollectionsActions)
                    {
                        action.Invoke(services);
                    }

                    var serviceProvider = services.BuildServiceProvider();

                    if(_cmsConfigUpdateAction != null)
                    {
                        var cmsConfig = serviceProvider.GetRequiredService<CMSConfig>();
                        _cmsConfigUpdateAction(cmsConfig);
                    }
                });

            base.ConfigureWebHost(builder);
        }

        /// <summary>
        /// Replace the existing CMS config with a new instance
        /// </summary>
        /// <param name="cmsConfig"></param>
        /// <returns></returns>
        public MASWebApplicationFactory WithCMSConfig(CMSConfig cmsConfig)
        {
            serviceCollectionsActions.Add(services => services.ReplaceService(cmsConfig));
            return this;
        }

        /// <summary>
        /// Update the existing CMS config via an action. Useful for overriding a specific property.
        /// </summary>
        /// <param name="action">The action to execute</param>
        /// <returns>The current MASWebApplicationFactory instance</returns>
        public MASWebApplicationFactory WithCMSConfig(Action<CMSConfig> action)
        {
            _cmsConfigUpdateAction = action;
            return this;
        }

        public MASWebApplicationFactory WithImplementation<TService>(TService implementation)
        {
            serviceCollectionsActions.Add(services => services.ReplaceService(implementation));
            return this;
        }

        public MASWebApplicationFactory WithImplementations<TService1, TService2>(TService1 implementation1, TService2 implementation2)
        {
            return WithImplementation(implementation1)
                .WithImplementation(implementation2);
        }

        public MASWebApplicationFactory WithImplementations<TService1, TService2, TService3>(TService1 implementation1, TService2 implementation2, TService3 implementation3)
        {
            return WithImplementation(implementation1)
                .WithImplementation(implementation2)
                .WithImplementation(implementation3);
        }
    }
}
