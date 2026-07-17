using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backend.Services
{
    /// <summary>
    /// SQLite pastreaza DateTime fara informatia de fus orar; toate datele noastre
    /// sunt UTC, asa ca la serializare fortam sufixul Z ca browserul sa le interpreteze corect.
    /// </summary>
    public class UtcDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetDateTime();
            return value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var utc = value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),
                _ => DateTime.SpecifyKind(value, DateTimeKind.Utc),
            };
            writer.WriteStringValue(utc.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'"));
        }
    }
}
