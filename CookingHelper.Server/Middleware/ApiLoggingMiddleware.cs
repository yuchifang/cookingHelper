using System.Text.Json;
using CookingHelper.Data;
using CookingHelper.LineDto;
using CookingHelper.Service;

namespace CookingHelper.Middleware;

public class ApiLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ApiLogService _apiLogService;

    public ApiLoggingMiddleware(RequestDelegate next, ApiLogService apiLogService)
    {
        _next = next;
        _apiLogService = apiLogService;
    }

    // Y軸 次數=> 在當天有使用的人數
    // X軸 時間=>
    // 操作不同功能呈現不同圖表

    public async Task InvokeAsync(HttpContext context)
    {
        //! UTC+0 確認
        var utcNowDate = DateTime.UtcNow;
        var sTimestamp = new DateTimeOffset(utcNowDate).ToUnixTimeSeconds();

        context.Request.EnableBuffering();
        string UserId;
        using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
        {
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
            var requestBody = JsonSerializer.Deserialize<WebhookRequestBodyDto>(body);
            if (requestBody != null && requestBody.Events != null && requestBody.Events.Count != 0)
            {
                UserId = requestBody!.Events[0].Source!.UserId!;
            }
            else
            {
                return;
            }
        }

        await _next(context);

        _apiLogService.AddLog(UserId, sTimestamp);
    }
}
