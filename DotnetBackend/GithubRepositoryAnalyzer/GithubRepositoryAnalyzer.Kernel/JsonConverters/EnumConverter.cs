using System.Text.Json;
using System.Text.Json.Serialization;

namespace GithubRepositoryAnalyzer.Kernel.JsonConverters;

public class EnumConverter<T> : JsonConverter<T>
{
    /// <inheritdoc />
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var type = typeof(T).IsGenericType ? typeof(T).GetGenericArguments().First() : typeof(T);

        if (!type.IsEnum)
        {
            throw new JsonException($"{type.FullName} is not supported.");
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();

            if (Enum.TryParse(type, value, out object result))
            {
                return (T)result;
            }
            else
            {
                throw new JsonException($"Can't convert {value} into {type.FullName}");
            }
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            return (T)(object)reader.GetInt32();
        }

        throw new NotSupportedException($"{reader.TokenType}");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(Convert.ToInt32(value));
    }
}