using System.Text.Json;
using System.Text.Json.Serialization;

namespace CookingHelper.ProviderGroup
{
    public class JsonProvider
    {
        // 將值轉為 json字串
        private JsonSerializerOptions serializeOption = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        // Json轉為特定的字串
        private static JsonSerializerOptions deserializeOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        public string Serialize<T>(T obj)
        {
            return JsonSerializer.Serialize(obj, serializeOption);
        }

        public T Deserialize<T>(string str)
        {
            return JsonSerializer.Deserialize<T>(str, deserializeOptions);
        }
    }
}