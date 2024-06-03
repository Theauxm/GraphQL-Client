using GraphQL.Client.Abstractions.Websocket;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using GraphQLParser.AST;

namespace GraphQL;

public class GraphQLClientConfiguration : IGraphQLClientConfiguration
{
    public static GraphQLClientConfiguration Create(
        Uri baseAddress,
        IGraphQLClientExecutor? clientExecutor = null,
        IGraphQLWebsocketJsonSerializer? jsonSerializer = null,
        GraphQLHttpClientOptions? graphQLHttpClientOptions = null,
        bool disposeHttpClient = false,
        bool validateAssemblies = false,
        HttpClient? httpClient = null)
    {
        httpClient ??= new HttpClient()
        {
            BaseAddress = baseAddress
        };

        jsonSerializer ??= new SystemTextJsonSerializer();

        graphQLHttpClientOptions ??= new GraphQLHttpClientOptions();
        
        return new GraphQLClientConfiguration
        {
            BaseAddress = baseAddress,
            HttpClient = httpClient,
            ClientExecutor = clientExecutor,
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

    public IGraphQLWebsocketJsonSerializer JsonSerializer { get; set; }

    public GraphQLHttpClientOptions GraphQLClientOptions { get; set; }

    public IGraphQLClientExecutor? ClientExecutor { get; set; }

    public bool ValidateAssemblies { get; set; }
    
    public GraphQLHttpClient GraphQLHttpClient { get; set; }

    public HttpClient HttpClient { get; set; }

    public bool DisposeHttpClient { get; set; }
}