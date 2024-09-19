namespace CookingHelper.LineDto
{
    public class ReplyMessageRequestDto<T>
    {
        public string ReplyToken { get; set; } = default!;
        public List<T> Messages { get; set; } = default!;
        public bool? NotificationDisabled { get; set; }
    }
}
