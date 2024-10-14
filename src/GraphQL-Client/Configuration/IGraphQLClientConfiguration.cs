using System.Text.Json;
using GraphQL.Client.Abstractions.Websocket;
using GraphQL.Client.Http;

namespace GraphQL;

public interface IGraphQLClientConfiguration
{
    public IGraphQLWebsocketJsonSerializer WebsocketJsonSerializer { get; }

    public JsonSerializerOptions JsonSerializerOptions { get; }

    public GraphQLHttpClientOptions GraphQLClientOptions { get; }

    public bool ValidateAssemblies { get; }

    public GraphQLHttpClient GraphQLHttpClient { get; }

    public HttpClient HttpClient { get; }

    public bool DisposeHttpClient { get; }
    
    public bool RemoveSubscriptionsFromSchema { get; }
}
