using System.Text.Json;
using GraphQL.Client.Abstractions.Websocket;
using GraphQL.Client.Http;

namespace GraphQL;

public interface IGraphQLClientConfiguration
{
    public Uri BaseAddress { get; set; }

    public IGraphQLWebsocketJsonSerializer WebsocketJsonSerializer { get; set; }
    
    public JsonSerializerOptions JsonSerializerOptions { get; set; }
    
    public GraphQLHttpClientOptions GraphQLClientOptions { get; set; }

    public IGraphQLClientExecutor? ClientExecutor { get; set; }

    public bool ValidateAssemblies { get; set; }
    
    public GraphQLHttpClient GraphQLHttpClient { get; internal set; }
    
    public HttpClient HttpClient { get; set; }
    
    public bool DisposeHttpClient { get; set; }
}