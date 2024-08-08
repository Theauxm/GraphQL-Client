using System.Text.Json;
using GraphQL.Client.Abstractions.Websocket;
using GraphQL.Client.Http;

namespace GraphQL;

public class GraphQLClientConfiguration : IGraphQLClientConfiguration
{
    public GraphQLClientConfiguration(
        Uri baseAddress,
        IGraphQLWebsocketJsonSerializer jsonSerializer,
        GraphQLHttpClientOptions graphQLHttpClientOptions,
        JsonSerializerOptions jsonSerializerOptions,
        bool disposeHttpClient,
        bool validateAssemblies,
        HttpClient httpClient
    )
    {
        HttpClient = httpClient;
        HttpClient.BaseAddress = baseAddress;

        JsonSerializerOptions = jsonSerializerOptions;
        GraphQLClientOptions = graphQLHttpClientOptions;
        WebsocketJsonSerializer = jsonSerializer;
        ValidateAssemblies = validateAssemblies;
        DisposeHttpClient = disposeHttpClient;

        GraphQLHttpClient = new GraphQLHttpClient(
            serializer: jsonSerializer,
            options: graphQLHttpClientOptions,
            httpClient: httpClient
        );
    }

    public IGraphQLWebsocketJsonSerializer WebsocketJsonSerializer { get; init; }

    public JsonSerializerOptions JsonSerializerOptions { get; init; }

    public GraphQLHttpClientOptions GraphQLClientOptions { get; init; }

    public bool ValidateAssemblies { get; init; }

    public GraphQLHttpClient GraphQLHttpClient { get; }

    public HttpClient HttpClient { get; init; }

    public bool DisposeHttpClient { get; init; }
}
