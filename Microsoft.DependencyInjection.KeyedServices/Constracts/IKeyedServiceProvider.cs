namespace Microsoft.DependencyInjection.KeyedServices.Contracts
{
    /// <summary>
    /// Provides registered services under the same interface, but different key.
    /// </summary>
    /// <typeparam name="TService">Service contract type</typeparam>
    /// <typeparam name="TKey">Type of key we distinguish implementation types with</typeparam>
    public interface IKeyedServiceProvider<out TService, in TKey>
    {
        TService GetKeyedService(TKey key);
    }
}
