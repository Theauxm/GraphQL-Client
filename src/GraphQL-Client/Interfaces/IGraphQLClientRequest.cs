using System.Text.Json;

namespace GraphQL;

public interface IGraphQLClientRequest<TOut> : IGenericGraphQLClientRequest
{
    public TOut GetNestedResponse(JsonElement response)
    {
        if (IsNested is false)
            throw new ArgumentNullException(nameof(NestedLocation));

        var resultData = NestedLocation!
            .Aggregate(response, (current, property) => current.GetProperty(property))
            .GetRawText();

        return JsonSerializer.Deserialize<TOut>(resultData);
    }
}