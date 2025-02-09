using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace StarWarsProgressBarIssueTracker.TestHelpers.Extensions;

public static class ServiceCollectionMockExtensions
{
    public static IServiceCollection VerifyServiceRegistered(this IServiceCollection serviceCollectionMock,
        Type interfaceType,
        Type implementationType, ServiceLifetime lifetime)
    {
        serviceCollectionMock.Received(1).Add(Arg.Is<ServiceDescriptor>(descriptor =>
                descriptor.ImplementationType == implementationType &&
                descriptor.ServiceType == interfaceType &&
                descriptor.Lifetime == lifetime));

        return serviceCollectionMock;
    }

    public static IServiceCollection VerifyServiceRegistered(this IServiceCollection serviceCollectionMock,
        Type implementationType, ServiceLifetime lifetime)
    {
        serviceCollectionMock.Received(1).Add(Arg.Is<ServiceDescriptor>(descriptor =>
            descriptor.ImplementationType == implementationType &&
            descriptor.ServiceType == implementationType &&
            descriptor.Lifetime == lifetime));

        return serviceCollectionMock;
    }
}
