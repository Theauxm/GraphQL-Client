using System.Text.Json;
using System.Text.Json.Serialization;
using GraphQL.Client.Http;
using GraphQL.IntrospectionModel;
using GraphQL.IntrospectionModel.SDL;
using GraphQL.Types;
using GraphQLParser.Visitors;

namespace GraphQL;

public static class GraphQLClientConfigurationExtensions
{
    public static ISchema IntrospectSchema(this IGraphQLClientConfiguration configuration) =>
        FetchSchemaJson(configuration).AsSchema(configuration);

    public static JsonElement FetchSchemaJson(IGraphQLClientConfiguration configuration)
    {
        var request = new GraphQLHttpRequest(IntrospectionQuery.Classic);

        var response = Task.Run(
            async () => await configuration.GraphQLHttpClient.SendQueryAsync<JsonElement>(request)
        ).Result;

        if (response.Errors is not null)
        {
            throw new Exception(
                $"Could not introspect ({configuration.HttpClient.BaseAddress}). Got the following errors: ({response.Errors})"
            );
        }

        return response.Data;
    }

    public static ISchema AsSchema(this JsonElement schemaJson, IGraphQLClientConfiguration configuration)
    {
        var schemaResponse = schemaJson.Deserialize<GraphQLData>(
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            }
        );

        if (schemaResponse?.__Schema is null)
        {
            throw new Exception(
                $"Could not get data from __schema introspection. Data likely came back null. Schema: ({schemaJson.GetRawText()})"
            );
        }

        if (configuration.RemoveSubscriptionsFromSchema)
            schemaResponse.__Schema.SubscriptionType = null;

        var converter = new ASTConverter();
        var document = converter.ToDocument(schemaResponse.__Schema);
        var printer = new SDLPrinter();
        var sdl = printer.Print(document);

        return Schema.For(sdl);
    }
}
