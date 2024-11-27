using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace StarWarsProgressBarIssueTracker.TestHelpers.Extensions;

public static class ServiceCollectionMockExtensions
{
    public static Mock<IServiceCollection> VerifyServiceRegistered(this Mock<IServiceCollection> serviceCollectionMock,
        Type interfaceType,
        Type implementationType, ServiceLifetime lifetime)
    {
        serviceCollectionMock.Verify(
            service => service.Add(It.Is<ServiceDescriptor>(descriptor =>
                descriptor.ImplementationType == implementationType &&
                descriptor.ServiceType == interfaceType &&
                descriptor.Lifetime == lifetime)), Times.Once);

        return serviceCollectionMock;
    }

    public static Mock<IServiceCollection> VerifyServiceRegistered(this Mock<IServiceCollection> serviceCollectionMock,
        Type implementationType, ServiceLifetime lifetime)
    {
        serviceCollectionMock.Verify(
            service => service.Add(It.Is<ServiceDescriptor>(descriptor =>
                descriptor.ImplementationType == implementationType &&
                descriptor.ServiceType == implementationType &&
                descriptor.Lifetime == lifetime)), Times.Once);

        return serviceCollectionMock;
    }
}
