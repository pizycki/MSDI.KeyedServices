using Microsoft.DependencyInjection.KeyedServices;
using Microsoft.DependencyInjection.KeyedServices.Contracts;
using System.Collections.Generic;

namespace Microsoft.DependencyInjection.KeyedServices.Tests
{
    #region FooBarQux

    public static class ContractKeys
    {
        public const string Foo = "Foo";
        public const string Bar = "Bar";
    }

    public interface IContract { }

    public class Foo : IContract { }

    public class Bar : IContract { }

    public class Qux
    {
        private readonly IKeyedServiceProvider<IContract, string> _implProvider;
        public Qux(IKeyedServiceProvider<IContract, string> implProvider)
        {
            _implProvider = implProvider;
        }

        public IEnumerable<IContract> GetImplementations()
        {
            yield return _implProvider.GetKeyedService(ContractKeys.Foo);
            yield return _implProvider.GetKeyedService(ContractKeys.Bar);
        }
    }

    #endregion

    #region AlfaBravoCharlie



    #endregion
}
