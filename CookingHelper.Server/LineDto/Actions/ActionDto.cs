namespace CookingHelper.LineDto;
public class ActionDto
{
    public string Type { get; set; } = default!;
    public string? Label { get; set; }

    public string? Data { get; set; } = default!;

    public string? DisplayText { get; set; }
    public string? InputOption { get; set; }

    public string? FillInText { get; set; }

    public string? Text { get; set; }

    public string? Uri { get; set; }

    public UriActionAltUriDto? AltUri { get; set; }
    public string? Mode { get; set; }
    public string? Initial { get; set; }
    public string? Max { get; set; }

    public string? Min { get; set; }
    // rich menu switch action
    public string? RichMenuAliasId { get; set; }
}
public class UriActionAltUriDto
{
    public string? Desktop { get; set; }
}