using System.Reflection;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQL;

public static class ServiceExtensions
{
    public static IServiceCollection AddGraphQLClientServices(
        this IServiceCollection services,
        GraphQLClientConfiguration? options = null,
        params Assembly[] assemblies)
    {
        options ??= new GraphQLClientConfiguration
        {
            BaseAddress = new Uri("https://legidex.constituentvoice.us/graphql/"),
            JsonSerializer = new SystemTextJsonSerializer(),
            GraphQLClientOptions = new GraphQLHttpClientOptions(),
            ValidateAssemblies = true,
            CustomClientService = null
        };

        var validator = new GraphQLClientValidator(options);

        if (options.ValidateAssemblies)
            validator.ValidateAssemblies(assemblies);

        options.CustomClientService ??= new GraphQLClientExecutor(validator, options);

        return services
            .AddSingleton<IGraphQLClientConfiguration>(options)
            .AddSingleton<IGraphQLClientValidator>(validator)
            .AddSingleton<IGraphQLClientExecutor>(options.CustomClientService);
    }
}