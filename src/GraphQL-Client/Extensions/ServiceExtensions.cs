using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQL;

public static class ServiceExtensions
{
    public static IServiceCollection AddGraphQLClientServices(
        this IServiceCollection services,
        GraphQLClientConfiguration options,
        params Assembly[] assemblies)
    {
        var validator = new GraphQLClientValidator(options);

        if (options.ValidateAssemblies)
            validator.ValidateAssemblies(assemblies);

        options.ClientExecutor ??= new GraphQLClientExecutor(validator, options);

        return services
            .AddSingleton<IGraphQLClientConfiguration>(options)
            .AddSingleton<IGraphQLClientValidator>(validator)
            .AddSingleton<IGraphQLClientExecutor>(options.ClientExecutor);
    }
}