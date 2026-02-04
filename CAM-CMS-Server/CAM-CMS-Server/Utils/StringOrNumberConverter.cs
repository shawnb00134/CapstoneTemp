using System.Text.Json;
using System.Text.Json.Serialization;

namespace CAMCMSServer.Utils;

public class StringOrNumberConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString();
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetDouble().ToString();
        }
        throw new JsonException($"Unexpected token parsing string or number. Token: {reader.TokenType}");
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        if (double.TryParse(value, out double number))
        {
            writer.WriteNumberValue(number);
        }
        else
        {
            writer.WriteStringValue(value);
        }
    }
}