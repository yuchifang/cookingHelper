namespace CookingHelper.Enum;

public static class MessageTypeEnum
{
    public const string Text = "text";
    public const string Postback = "postback";

    public const string Flex = "flex";
}

public static class ActionTypeEnum
{
    public const string Message = "message";
    public const string Postback = "postback";
}

public static class FlexContainerTypeEnum
{
    public const string Bubble = "bubble";
    public const string Carousel = "carousel";
}

public static class FlexComponentTypeEnum
{
    public const string Box = "box";
    public const string Text = "text";

    public const string Separator = "separator";
    public const string Button = "button";
}

public static class FlexComponentLayoutTypeEnum
{
    public const string Vertical = "vertical";
    public const string Horizontal = "horizontal";
}
