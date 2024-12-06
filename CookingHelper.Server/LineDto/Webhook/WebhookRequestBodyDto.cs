using System.Text.Json.Serialization;

namespace CookingHelper.LineDto;

public class WebhookRequestBodyDto
{
    public string? Destination { get; set; }

    [JsonPropertyName("events")]
    public List<WebhookEventDto> Events { get; set; } = default!;
}
