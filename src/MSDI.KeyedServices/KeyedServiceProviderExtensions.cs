using MSDI.KeyedServices.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace MSDI.KeyedServices
{
    /// <summary>
    /// Set of extensions for <see cref="IServiceCollection"/> to ease use of <see cref="KeyedServiceProvider{TService, TKey}"/> type.
    /// </summary>
    public static class KeyedServiceProviderExtensions
    {
        /// <summary>
        /// Registers type in <see cref="KeyedServiceCollection{TService, TKey}"/>.
        /// The type should be registered itself seperetly or via passed in function.
        /// All required supporting services (i.e.: <see cref="KeyedServiceProvider{TService, TKey}" /> or <see cref="KeyedServiceCollection{TService, TKey}"/> will be checked and registered if needed.
        /// </summary>
        /// <typeparam name="TService">Contract of implementation type we'd like to register</typeparam>
        /// <typeparam name="TImplementation">Type of implementation we'd like to register</typeparam>
        /// <typeparam name="TKey">Type of key we will distinguish implementations with</typeparam>
        /// <param name="services">Collection of services used to build instance of <see cref="IServiceProvider"/>.</param>
        /// <param name="key">Key to distiguish implementation</param>
        /// <param name="registerImplementation">The way we'd like our implementation type to be registered as</param>
        /// <returns>Reference to collection passed in as argument</returns>
        public static IServiceCollection AddKeyedService<TService, TImplementation, TKey>(
            this IServiceCollection services,
            TKey key,
            Action<IServiceCollection> registerImplementation = null)
        {
            var implementationType = typeof(TImplementation);

            // Register type by key
            var collection = services.GetKeyedServiceCollection<TService, TKey>() ?? services.AddKeyedServiceCollection<TService, TKey>();
            collection.Add(key, implementationType);

            // Register type if the register action is passed in
            registerImplementation?.Invoke(services);

            if (!services.IsKeyedServiceProviderRegistered<TService, TKey>())
                services.AddSingleton<IKeyedServiceProvider<TService, TKey>, KeyedServiceProvider<TService, TKey>>();

            return services;
        }

        /// <summary>
        /// Retrieves service descriptor with registered <see cref="KeyedServiceCollection{TService, TKey}"/>.
        /// </summary>
        /// <returns>When descriptor is not found, returns null.</returns>
        public static ServiceDescriptor GetKeyedServiceCollectionDescriptor<TService, TKey>(this IServiceCollection services) =>
            services.FirstOrDefault(sd => sd.ServiceType == typeof(KeyedServiceCollection<TService, TKey>));

        /// <summary>
        /// Registers <see cref="KeyedServiceCollection{TService, TKey}"/> instance in <see cref="IServiceCollection"/> as singleton.
        /// </summary>
        /// <typeparam name="TService">Implementation contract of registrations kept in collection</typeparam>
        /// <typeparam name="TKey">Type of key we will distinguish implementations with</typeparam>
        /// <param name="services">Services collection</param>
        /// <returns>Registered instance</returns>
        public static KeyedServiceCollection<TService, TKey> AddKeyedServiceCollection<TService, TKey>(
            this IServiceCollection services)
        {
            var collection = new KeyedServiceCollection<TService, TKey>();
            services.AddSingleton(collection);
            return collection;
        }

        /// <summary>
        /// Searches for first registered instance of <see cref="KeyedServiceCollection{TService, TKey}"/>.
        /// </summary>
        /// <typeparam name="TService">Implementation contract of registrations kept in collection</typeparam>
        /// <typeparam name="TKey">Type of key we will distinguish implementations with</typeparam>
        /// <param name="services">Services collection</param>
        /// <returns>Registered instance</returns>
        public static KeyedServiceCollection<TService, TKey> GetKeyedServiceCollection<TService, TKey>(this IServiceCollection services) =>
            (KeyedServiceCollection<TService, TKey>)GetKeyedServiceCollectionDescriptor<TService, TKey>(services)?.ImplementationInstance;

        /// <summary>
        /// Werifies that any service is registered as <see cref="IKeyedServiceProvider{TService, TKey}"/>.
        /// </summary>
        /// <typeparam name="TService">Implementation contract</typeparam>
        /// <typeparam name="TKey">Type of key we will distinguish implementations with</typeparam>
        /// <param name="services">Services collection</param>
        /// <returns>True or false</returns>
        public static bool IsKeyedServiceProviderRegistered<TService, TKey>(this IServiceCollection services) =>
            services.Any(sd => sd.ServiceType == typeof(IKeyedServiceProvider<TService, TKey>));


    }
}
