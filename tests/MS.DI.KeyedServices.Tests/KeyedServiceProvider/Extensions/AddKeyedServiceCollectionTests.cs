using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;

namespace MS.DI.KeyedServices.Tests.KeyedServiceProvider.Extensions
{
    public class AddKeyedServiceCollectionTests
    {
        [Fact]
        public void adds_ksc_as_singleton()
        {
            var services = new ServiceCollection();

            var ksc = services.AddKeyedServiceCollection<IContract, string>();

            ServiceDescriptor descriptor = services.First();
            descriptor.ServiceType.Should().Be(typeof(KeyedServiceCollection<IContract, string>));
            descriptor.ImplementationInstance?.Should().Be(ksc);
            descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
        }

        [Fact]
        public void adds_ksc_and_can_retrieve_instance_from_service_collection()
        {
            var services = new ServiceCollection();

            var ksc = services.AddKeyedServiceCollection<IContract, string>();

            ServiceDescriptor descriptor = services.First();
            descriptor.ImplementationInstance.Should().NotBeNull();
        }
    }
}
