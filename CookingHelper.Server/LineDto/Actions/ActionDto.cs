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
}

public static class PostbackInputOptionEnum
{
    public const string CloseRichMenu = "closeRichMenu";
    public const string OpenRichMenu = "openRichMenu";
    public const string OpenKeyboard = "openKeyboard";
    public const string OpenVoice = "openVoice";
}
