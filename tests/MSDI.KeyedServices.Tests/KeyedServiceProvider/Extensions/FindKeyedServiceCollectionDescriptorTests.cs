using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MS.DI.KeyedServices.Tests
{
    public class GetKeyedServiceCollectionDescriptorTests
    {
        [Fact]
        public void can_find_service_descriptor_for_ksp_in_service_collection()
        {
            var services = new ServiceCollection();
            services.AddSingleton(new KeyedServiceCollection<IContract, string>());

            var ksp = services.GetKeyedServiceCollectionDescriptor<IContract, string>();

            ksp.Should().NotBeNull();
        }

        [Fact]
        public void returns_null_if_descriptor_is_not_found_for_ksp_in_collection()
        {
            var services = new ServiceCollection();

            var ksp = services.GetKeyedServiceCollectionDescriptor<IContract, string>();

            ksp.Should().BeNull();
        }
    }
}
