using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQL;

public static class ServiceExtensions
{
    public static IServiceCollection AddGraphQLClientServices(
        this IServiceCollection services,
        Uri baseAddress,
        Action<GraphQLClientConfigurationBuilder>? options = null,
        params Assembly[] assemblies)
    {
        // Create Builder to be used after Options are invoked
        var builder = new GraphQLClientConfigurationBuilder(baseAddress);
        
        // Options able to be null since all values have defaults
        options?.Invoke(builder);

        var configuration = builder.Configuration;
        
        var validator = new GraphQLClientValidator(configuration);

        if (configuration.ValidateAssemblies)
            validator.ValidateAssemblies(assemblies);

        var executor = new GraphQLClientExecutor(validator, configuration);

        return services
            .AddSingleton<IGraphQLClientConfiguration>(configuration)
            .AddSingleton<IGraphQLClientValidator>(validator)
            .AddSingleton<IGraphQLClientExecutor>(executor);
    }
}