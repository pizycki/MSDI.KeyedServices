using FluentAssertions;
using Microsoft.DependencyInjection.KeyedServices.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.DependencyInjection.KeyedServices.Tests.KeyedServiceProvider.Extensions
{
    public class AddKeyedServiceTests
    {
        [Fact]
        public void adds_KeyedServiceProvider_to_service_collection()
        {
            var services = new ServiceCollection();

            services.AddKeyedService<IContract, Foo, string>(ContractKeys.Foo);

            var provider = services.BuildServiceProvider();
            var keyedProvider = provider.GetService<IKeyedServiceProvider<IContract, string>>();
            keyedProvider.Should().NotBeNull();
        }

        [Fact]
        public void adds_KeyedServiceCollection()
        {
            var services = new ServiceCollection();

            services.AddKeyedService<IContract, Foo, string>(ContractKeys.Foo);

            var provider = services.BuildServiceProvider();
            var keyedServiceCollection = provider.GetService<KeyedServiceCollection<IContract, string>>();
            keyedServiceCollection.Should().NotBeNull();
        }

        [Fact]
        public void adds_registered_service()
        {
            var services = new ServiceCollection();

            services.AddKeyedService<IContract, Foo, string>(ContractKeys.Foo, srv => srv.AddTransient<Foo>());

            var provider = services.BuildServiceProvider();
            var service = provider.GetService<Foo>();
            service.Should().NotBeNull();
        }

        [Fact]
        public void adds_and_returns_the_same_services_instance()
        {
            var services = new ServiceCollection();

            var returnedServices = services.AddKeyedService<IContract, Foo, string>(ContractKeys.Foo, srv => srv.AddTransient<Foo>());

            ReferenceEquals(services, returnedServices).Should().BeTrue();
        }
    }
}
