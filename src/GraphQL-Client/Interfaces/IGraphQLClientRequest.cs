using System.Text.Json;

namespace GraphQL;

public interface IGraphQLClientRequest<out TOut> : IGenericGraphQLClientRequest
{
    public TOut GetNestedResponse(JsonElement response, JsonSerializerOptions options)
    {
        if (IsNested is false)
            throw new ArgumentNullException(nameof(NestedLocation));

        var resultData = NestedLocation!
            .Aggregate(response, (current, property) => current.GetProperty(property))
            .GetRawText();

        return JsonSerializer.Deserialize<TOut>(resultData, options) ?? throw new InvalidOperationException();
    }
}