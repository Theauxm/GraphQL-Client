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
    public static ISchema IntrospectSchema(this IGraphQLClientConfiguration configuration)
        => FetchSchemaJson(configuration)
            .AsSchema();

    private static JsonElement FetchSchemaJson(IGraphQLClientConfiguration configuration)
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = configuration.BaseAddress;

        var graphQLClient = new GraphQLHttpClient(
            serializer: configuration.JsonSerializer,
            options: configuration.GraphQLClientOptions,
            httpClient: httpClient);

        var request = new GraphQLHttpRequest(IntrospectionQuery.Classic);

        var response = Task.Run(async () =>
        {
            var result = await graphQLClient.SendQueryAsync<JsonElement>(request);

            graphQLClient.Dispose();

            return result;
        }).Result;

        if (response.Errors is not null)
        {
            throw new Exception(
                $"Could not introspect ({configuration.BaseAddress}). Got the following errors: ({response.Errors})");
        }

        return response.Data;
    }

    private static ISchema AsSchema(this JsonElement schemaJson)
    {
        var schemaResponse = schemaJson.Deserialize<GraphQLData>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        });

        if (schemaResponse.__Schema is null)
        {
            throw new Exception(
                $"Could not get data from __schema introspection. Data likely came back null. Schema: ({schemaJson.GetRawText()})");
        }

        var converter = new ASTConverter();
        var document = converter.ToDocument(schemaResponse.__Schema);
        var printer = new SDLPrinter();
        var sdl = printer.Print(document);

        return Schema.For(sdl);
    }
}