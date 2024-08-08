namespace GraphQL;

public interface IGenericGraphQLClientRequest
{
    public string Query { get; }

    public List<string>? NestedLocation { get; }

    public bool IsNested => NestedLocation is not null;
}
