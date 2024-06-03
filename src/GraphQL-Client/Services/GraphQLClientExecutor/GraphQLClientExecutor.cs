using GraphQL.Client.Http;
using GraphQLParser.AST;

namespace GraphQL;

public class GraphQLClientExecutor(
    IGraphQLClientValidator queryValidator,
    IGraphQLClientConfiguration graphQlClientConfiguration)
    : IGraphQLClientExecutor, IDisposable
{
    private readonly GraphQLHttpClient _graphQLClient = graphQlClientConfiguration.GraphQLHttpClient;

    public async Task<TReturn> Run<TReturn>(IGraphQLClientRequest<TReturn> request)
    {
        var operationType = await queryValidator.Validate(request.Query);

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
        _graphQLClient.Dispose();
        
        if (graphQlClientConfiguration.DisposeHttpClient)
            graphQlClientConfiguration.HttpClient?.Dispose();
            
    }
}