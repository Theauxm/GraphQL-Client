using System.Collections.Concurrent;
using System.Reflection;
using GraphQL.Execution;
using GraphQL.Types;
using GraphQL.Validation;
using GraphQLParser.AST;

namespace GraphQL;

public class GraphQLClientValidator(IGraphQLClientConfiguration graphQLClientConfiguration) : IGraphQLClientValidator
{
    public ISchema Schema { get; } = graphQLClientConfiguration.IntrospectSchema();

    private readonly DocumentValidator _validator = new();

    private readonly GraphQLDocumentBuilder _documentBuilder = new();

    private ConcurrentDictionary<string, OperationType> CachedQueries { get; } = new();

    public async Task<OperationType> Validate(string query)
    {
        if (CachedQueries.TryGetValue(query, out var cachedQuery))
            return cachedQuery;

        var document = _documentBuilder.Build(query);

        if (document.Definitions.FirstOrDefault() is not GraphQLOperationDefinition queryType)
            throw new Exception($"Could not find any query definitions in: ({query})");

        var validationOptions = new ValidationOptions
        {
            Schema = Schema,
            Document = document
        };

        var (validationResult, _) = await _validator.ValidateAsync(validationOptions);

        if (!validationResult.IsValid)
        {
            throw new Exception(
                $"GraphQL Query ({query}) is not valid with the given schema. Got the following errors: ({validationResult.Errors}");
        }

        CachedQueries[query] = queryType.Operation;

        return queryType.Operation;
    }

    public GraphQLClientValidator ValidateAssemblies(params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var queries = assembly
                .GetTypes()
                .Where(x => x.IsClass)
                .Where(x => typeof(IGenericGraphQLClientRequest).IsAssignableFrom(x))
                .ToList();

            foreach (var query in queries)
            {
                var instance = (IGenericGraphQLClientRequest)Activator.CreateInstance(query)!;

                var result = Task.Run(() => Validate(instance.Query)).Result;

                CachedQueries[instance.Query] = result;
            }
        }

        return this;
    }
}