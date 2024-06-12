namespace CookingHelper.LineDto;
public class WebhookRequestBodyDto
{
    public string? Destination { get; set; }

    public List<WebhookEventDto> Events { get; set; } = default!;
}
