using System.Reflection;

namespace GraphQL;

public static class GraphQLClientValidatorExtensions
{
    public static GraphQLClientValidator ValidateAssemblies(
        this GraphQLClientValidator validator,
        params Assembly[] assemblies
    )
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

                var result = Task.Run(() => validator.Validate(instance.Query)).Result;

                validator.CachedQueries[instance.Query] = result;
            }
        }

        return validator;
    }
}
