using System.Text.Json;
using System.Text.Json.Serialization;
using GraphQL.Client.Abstractions.Websocket;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using GraphQL.Utils.Converters;

namespace GraphQL;

public class GraphQLClientConfigurationBuilder(Uri baseAddress)
{
    public GraphQLClientConfiguration Build() =>
        new(
            baseAddress,
            WebsocketJsonSerializer,
            GraphQLClientOptions,
            JsonSerializerOptions,
            DisposeHttpClient,
            ValidateAssemblies,
            HttpClient
        );

    public IGraphQLWebsocketJsonSerializer WebsocketJsonSerializer { get; set; } =
        new SystemTextJsonSerializer();

    public JsonSerializerOptions JsonSerializerOptions { get; set; } =
        new()
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseUpper),
                new DateOnlyConverter()
            }
        };

    public GraphQLHttpClientOptions GraphQLClientOptions { get; set; } = new();

    public bool ValidateAssemblies { get; set; } = false;

    public HttpClient HttpClient { get; set; } = new();

    public bool DisposeHttpClient { get; set; } = false;
}
