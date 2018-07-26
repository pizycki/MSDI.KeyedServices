using System.Linq;
using FluentAssertions;
using Microsoft.DependencyInjection.KeyedServices.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.DependencyInjection.KeyedServices.Tests
{
    public class KeyedServiceProviderFacts
    {
        [Fact]
        public void service_provider_can_resolve_ksp()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddKeyedService<IContract, Foo, string>(ContractKeys.Foo);
            var provider = services.BuildServiceProvider();

            // Act and Assert
            provider.GetService<IKeyedServiceProvider<IContract, string>>().Should().NotBeNull();
        }

        [Fact]
        public void ksp_cannot_resolve_service_if_the_service_was_not_registered_itself()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddKeyedService<IContract, Foo, string>(ContractKeys.Foo);
            var provider = services.BuildServiceProvider();

            // Act
            var ksp = provider.GetService<IKeyedServiceProvider<IContract, string>>();
            var srv = ksp.GetKeyedService(ContractKeys.Foo);

            // Assert
            srv.Should().BeNull();
        }

        [Fact]
        public void ksp_can_resolve_service_if_the_service_has_been_registered_itself()
        {
            var services = new ServiceCollection();
            services.AddKeyedService<IContract, Foo, string>(ContractKeys.Foo, s => s.AddTransient<Foo>());
            var provider = services.BuildServiceProvider();

            // Act
            var ksp = provider.GetService<IKeyedServiceProvider<IContract, string>>();
            var srv = ksp.GetKeyedService(ContractKeys.Foo);

            // Assert
            srv.Should().NotBeNull();
        }

        [Fact]
        public void ksp_can_resolve_service_from_two_registered_implementations()
        {
            var services = new ServiceCollection();
            services.AddKeyedService<IContract, Foo, string>(ContractKeys.Foo, s => s.AddTransient<Foo>());
            services.AddKeyedService<IContract, Bar, string>(ContractKeys.Bar, s => s.AddTransient<Bar>());
            var provider = services.BuildServiceProvider();
            var ksp = provider.GetService<IKeyedServiceProvider<IContract, string>>();

            // Act and Assert
            ksp.GetKeyedService(ContractKeys.Foo).Should().NotBeNull().And.BeOfType<Foo>();
            ksp.GetKeyedService(ContractKeys.Bar).Should().NotBeNull().And.BeOfType<Bar>();
        }

        [Fact]
        public void ksp_can_be_injected_into_constructor()
        {
            var services = new ServiceCollection();
            services.AddTransient<Qux>();
            services.AddKeyedService<IContract, Foo, string>(ContractKeys.Foo, s => s.AddTransient<Foo>());
            services.AddKeyedService<IContract, Bar, string>(ContractKeys.Bar, s => s.AddTransient<Bar>());
            var provider = services.BuildServiceProvider();

            var qux = provider.GetService<Qux>();
            qux.Should().NotBeNull();
            qux.GetImplementations().ToList().ForEach(x => x.Should().NotBeNull());
        }
    }
}