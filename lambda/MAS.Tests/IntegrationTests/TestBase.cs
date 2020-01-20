using MAS.Tests.Infrastructure;
using System;
using Xunit.Abstractions;

namespace MAS.Tests.IntegrationTests
{
    public abstract class TestBase : IDisposable
    {
        protected readonly MASWebApplicationFactory _factory;
        protected readonly ITestOutputHelper _output;

        public TestBase(ITestOutputHelper output)
        {
            _factory = new MASWebApplicationFactory(output);
            _output = output;
        }

        public virtual void Dispose()
        {
            _factory.Dispose();
        }
    }
}
