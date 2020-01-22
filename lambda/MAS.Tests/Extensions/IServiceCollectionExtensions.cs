using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAS.Tests.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void ReplaceService<TService>(this IServiceCollection serviceCollection, TService implementation)
        {
            ServiceDescriptor descriptor =
                        new ServiceDescriptor(
                            typeof(TService), implementation);

            serviceCollection.Replace(descriptor);
        }
    }
}
