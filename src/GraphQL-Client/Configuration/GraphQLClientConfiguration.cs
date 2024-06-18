using System.Text.Json;
using System.Text.Json.Serialization;
using GraphQL.Client.Abstractions.Websocket;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using GraphQL.Utils.Converters;

namespace GraphQL;

public class GraphQLClientConfiguration : IGraphQLClientConfiguration
{
    public static GraphQLClientConfiguration Create(
        Uri baseAddress,
        IGraphQLClientExecutor? clientExecutor = null,
        IGraphQLWebsocketJsonSerializer? jsonSerializer = null,
        GraphQLHttpClientOptions? graphQLHttpClientOptions = null,
        JsonSerializerOptions? jsonSerializerOptions = null,
        bool disposeHttpClient = false,
        bool validateAssemblies = false,
        HttpClient? httpClient = null)
    {
        httpClient ??= new HttpClient();
        httpClient.BaseAddress = baseAddress;

        jsonSerializer ??= new SystemTextJsonSerializer();

        jsonSerializerOptions ??= new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseUpper),
                new DateOnlyConverter()
            }
        };

        graphQLHttpClientOptions ??= new GraphQLHttpClientOptions();
        
        return new GraphQLClientConfiguration
        {
            BaseAddress = baseAddress,
            HttpClient = httpClient,
            ClientExecutor = clientExecutor,
            JsonSerializerOptions = jsonSerializerOptions,
            GraphQLClientOptions = graphQLHttpClientOptions,
            WebsocketJsonSerializer = jsonSerializer,
            GraphQLHttpClient = new GraphQLHttpClient(
                serializer: jsonSerializer,
                options: graphQLHttpClientOptions,
                httpClient: httpClient
            ),
            DisposeHttpClient = disposeHttpClient,
            ValidateAssemblies = validateAssemblies
        };
    }

    private GraphQLClientConfiguration() { }

    public required Uri BaseAddress { get; set; }

    public IGraphQLWebsocketJsonSerializer WebsocketJsonSerializer { get; set; }
    
    public JsonSerializerOptions JsonSerializerOptions { get; set; }

    public GraphQLHttpClientOptions GraphQLClientOptions { get; set; }

    public IGraphQLClientExecutor? ClientExecutor { get; set; }

    public bool ValidateAssemblies { get; set; }
    
    public GraphQLHttpClient GraphQLHttpClient { get; set; }

    public HttpClient HttpClient { get; set; }

    public bool DisposeHttpClient { get; set; }
}