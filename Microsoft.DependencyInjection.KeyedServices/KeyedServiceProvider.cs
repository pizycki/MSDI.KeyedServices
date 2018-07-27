using Microsoft.DependencyInjection.KeyedServices.Contracts;
using System;

namespace Microsoft.DependencyInjection.KeyedServices
{
    /// <summary>
    /// Provides registered services under the same contract, but distinguished with key.
    /// Use <see cref="KeyedServiceProviderExtensions"/> to register keyed services.
    /// </summary>
    /// <typeparam name="TService">Service contract type</typeparam>
    /// <typeparam name="TKey">Type of key we distinguish services with</typeparam>
    public class KeyedServiceProvider<TService, TKey> : IKeyedServiceProvider<TService, TKey>
    {
        private readonly Func<TKey, TService> _lookUp;

        /// <summary>
        /// Creates instance <see cref="KeyedServiceCollection{TService, TKey}"/>.
        /// This is generaly meant to be used with <see cref="IServiceProvider"/>.
        /// The <see cref="KeyedServiceProvider{TService, TKey}"/> instance is registered with the first service of <see cref="TService"/> inside <see cref="KeyedServiceProviderExtensions.AddKeyedService{TService, TImplementation, TKey}(Extensions.DependencyInjection.IServiceCollection, TKey, Action{Extensions.DependencyInjection.IServiceCollection})"/>.
        /// </summary>
        /// <param name="serviceProvider">Instance of <see cref="IServiceProvider"/>. The <see cref="IServiceProvider"/> can injects itself.</param>
        /// <param name="implementations">Registered implementations collection under the same contract. The collection should be registered in services collection as singleton. You can do that with <see cref="KeyedServiceProviderExtensions.AddKeyedServiceCollection{TService, TKey}(Extensions.DependencyInjection.IServiceCollection)"/> extension.</param>
        public KeyedServiceProvider(IServiceProvider serviceProvider, KeyedServiceCollection<TService, TKey> implementations)
        {
            _lookUp = key =>
            {
                Type resolvedServiceType = implementations[key];
                return (TService)serviceProvider.GetService(resolvedServiceType);
            };
        }

        /// <summary>
        /// Looks up for implementation type assosiated with key and then resolves it within <see cref="IServiceProvider"/>.
        /// Throws exception when key is assosiated with any type.
        /// </summary>
        /// <param name="key">Key assosiated with implementation type</param>
        /// <returns>Resolved service or null if type is not registered properly.</returns>
        public TService GetKeyedService(TKey key) => _lookUp(key);
    }
}
