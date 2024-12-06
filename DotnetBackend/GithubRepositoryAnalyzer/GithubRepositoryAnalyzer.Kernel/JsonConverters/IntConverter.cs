using System.Text.Json;
using System.Text.Json.Serialization;

namespace GithubRepositoryAnalyzer.Kernel.JsonConverters;

public class IntConverter : JsonConverter<int>
{
    /// <inheritdoc />
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            return reader.TokenType switch
            {
                JsonTokenType.Number => reader.TryGetInt32(out int _)
                    ? reader.GetInt32()
                    : throw new JsonException("Value is not a valid int value"),
            };
        }

        var value = reader.GetString();

        if (int.TryParse(value, out int intValue))
        {
            return intValue;
        }

        throw new JsonException($"Value {value} is not a valid int value");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}