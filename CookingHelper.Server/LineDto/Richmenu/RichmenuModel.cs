using CookingHelper.LineDto;

namespace CookingHelper.LineDto;

public class RichMenuModel
{
    public string? RichMenuId { get; set; }
    public Size Size { get; set; } = default!;
    public bool Selected { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string ChatBarText { get; set; } = default!;
    public Area[] Areas { get; set; } = default!;
}

public class Size
{
    public int Width { get; set; }
    public int Height { get; set; }
}

public class Area
{
    public Bounds Bounds { get; set; } = default!;
    public ActionDto Action { get; set; } = default!;
}

public class Bounds
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}
