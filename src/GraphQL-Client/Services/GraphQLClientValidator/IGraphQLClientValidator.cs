using GraphQLParser.AST;

namespace GraphQL;

public interface IGraphQLClientValidator
{
    public Task<OperationType> Validate(string query);
}