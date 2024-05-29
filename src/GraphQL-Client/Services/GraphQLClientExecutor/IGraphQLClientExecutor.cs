namespace GraphQL;

public interface IGraphQLClientExecutor
{
    public Task<TReturn> Run<TReturn>(IGraphQLClientRequest<TReturn> request);
}