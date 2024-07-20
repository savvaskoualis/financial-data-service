using System.Text.Json;
using System.Text.Json.Serialization;

namespace FinancialDataService.Shared.Converters;

public class UnixEpochDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt64(out long value))
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(value).UtcDateTime;
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        long milliseconds = ((DateTimeOffset)value).ToUnixTimeMilliseconds();
        writer.WriteNumberValue(milliseconds);
    }
}