using GraphQL.Client.Abstractions.Websocket;
using GraphQL.Client.Http;

namespace GraphQL;

public interface IGraphQLClientConfiguration
{
    public Uri BaseAddress { get; set; }

    public IGraphQLWebsocketJsonSerializer JsonSerializer { get; set; }

    public GraphQLHttpClientOptions GraphQLClientOptions { get; set; }

    public IGraphQLClientExecutor? CustomClientService { get; set; }

    public bool ValidateAssemblies { get; set; }
}