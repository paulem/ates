using System.Text.Json.Nodes;
using Json.Schema;

namespace Ates.SchemaRegistry;

public static class SchemaValidator
{
    public static ValidationResult Validate(string @event, string domainName, string eventName, int eventVersion)
    {
        var schemaFilePath = Path.Combine(AppContext.BaseDirectory, "Schemas",
            domainName, eventName, $"{eventVersion}.json");

        if (!File.Exists(schemaFilePath))
            throw new InvalidOperationException("Schema file not exist");

        var schema = JsonSchema.FromFile(schemaFilePath);
        var evaluationResults = schema.Evaluate(JsonNode.Parse(@event));

        return evaluationResults is { IsValid: true }
            ? ValidationResult.Success
            : ValidationResult.Failure(
                string.Join("; ", evaluationResults.Errors?.Values ?? ArraySegment<string>.Empty));
    }
}