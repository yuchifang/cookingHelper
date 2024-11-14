public class ApiLog
{
    public int Id { get; set; } = default!;

    // UTC+0 Second
    public long LogTime { get; set; } = default!;
    public string UserId { get; set; } = default!;
}
