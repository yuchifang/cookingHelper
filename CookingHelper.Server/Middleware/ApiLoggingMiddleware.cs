using System.Text.Json;
using CookingHelper.Data;
using CookingHelper.LineDto;

public class ApiLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public ApiLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    // Y軸 次數=> 在當天有使用的人數
    // X軸 時間=>
    // 操作不同功能呈現不同圖表
    // todo 修改此 page
    public async Task InvokeAsync(HttpContext context)
    {
        var nowDate = DateTime.UtcNow;

        context.Request.EnableBuffering();
        string UserId;
        using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
        {
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
            var requestBody = JsonSerializer.Deserialize<WebhookRequestBodyDto>(body);
            UserId = requestBody!.Events[0].Source!.UserId!;
        }

        await _next(context);

        var apiLog = new ApiLog
        {
            // UTC+0
            LogTime = nowDate,
            UserId = UserId
        };

        using (var scope = context.RequestServices.CreateScope())
        {
            var service = scope.ServiceProvider;
            var dbContext = service.GetRequiredService<UserListDbContext>();
            dbContext.ApiLog.Add(apiLog);
            await dbContext.SaveChangesAsync();
        }
    }
}
