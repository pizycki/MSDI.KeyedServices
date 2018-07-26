using Microsoft.DependencyInjection.KeyedServices.Contracts;
using System;

namespace Microsoft.DependencyInjection.KeyedServices
{
    public class KeyedServiceProvider<TService, TKey> : IKeyedServiceProvider<TService, TKey>
    {
        private readonly Func<TKey, TService> _lookUp;

        public KeyedServiceProvider(IServiceProvider serviceProvider, KeyedServiceCollection<TService, TKey> implementations)
        {
            _lookUp = key =>
            {
                Type resolvedServiceType = implementations[key];
                return (TService)serviceProvider.GetService(resolvedServiceType);
            };
        }

        public TService GetKeyedService(TKey key) => _lookUp(key);
    }
}
