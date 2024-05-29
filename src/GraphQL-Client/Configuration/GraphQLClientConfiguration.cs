using GraphQL.Client.Abstractions.Websocket;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;

namespace GraphQL;

public class GraphQLClientConfiguration : IGraphQLClientConfiguration
{
    public required Uri BaseAddress { get; set; }

    public IGraphQLWebsocketJsonSerializer JsonSerializer { get; set; } = new SystemTextJsonSerializer();

    public GraphQLHttpClientOptions GraphQLClientOptions { get; set; } = new();

    public IGraphQLClientExecutor? CustomClientService { get; set; }

    public bool ValidateAssemblies { get; set; } = false;
}