namespace CookingHelper.Enum;

public struct MessageTypeEnum
{
    public const string Text = "text";
    public const string Postback = "postback";

    public const string Flex = "flex";
}

public struct ActionTypeEnum
{
    public const string Message = "message";
    public const string Postback = "postback";
}

public struct FlexContainerTypeEnum
{
    public const string Bubble = "bubble";
    public const string Carousel = "carousel";
}

public struct FlexComponentTypeEnum
{
    public const string Box = "box";
    public const string Text = "text";

    public const string Separator = "separator";
    public const string Button = "button";

    public const string Image = "image";
}

public struct FlexComponentLayoutTypeEnum
{
    public const string Vertical = "vertical";
    public const string Horizontal = "horizontal";
}
