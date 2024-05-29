using GraphQL.Client.Http;
using GraphQLParser.AST;

namespace GraphQL;

public class GraphQLClientExecutor : IGraphQLClientExecutor, IDisposable
{
    private readonly IGraphQLClientValidator _queryValidator;

    private readonly HttpClient _httpClient;

    private readonly GraphQLHttpClient _graphQLClient;

    public GraphQLClientExecutor(
        IGraphQLClientValidator queryValidator,
        IGraphQLClientConfiguration graphQlClientConfiguration)
    {
        _queryValidator = queryValidator;

        _httpClient = new HttpClient();
        _httpClient.BaseAddress = graphQlClientConfiguration.BaseAddress;

        _graphQLClient = new GraphQLHttpClient(
            serializer: graphQlClientConfiguration.JsonSerializer,
            options: graphQlClientConfiguration.GraphQLClientOptions,
            httpClient: _httpClient);
    }

    public async Task<TReturn> Run<TReturn>(IGraphQLClientRequest<TReturn> request)
    {
        var operationType = await _queryValidator.Validate(request.Query);

        var httpRequest = new GraphQLHttpRequest(request.Query);

        var response = operationType switch
        {
            OperationType.Query => await _graphQLClient.SendQueryAsync<dynamic>(httpRequest),
            OperationType.Mutation => await _graphQLClient.SendMutationAsync<dynamic>(httpRequest),
            OperationType.Subscription => throw new ArgumentException("Subscriptions Not Supported."),
            _ => throw new ArgumentOutOfRangeException()
        };

        return request.IsNested
            ? request.GetNestedResponse(response.Data)
            : response.Data;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        _graphQLClient.Dispose();
    }
}