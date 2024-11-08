using CookingHelper.Data;

namespace CookingHelper.Service;

public class ApiLogBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ApiLogService _apiLogService;

    public ApiLogBackgroundService(IServiceProvider serviceProvider, ApiLogService apiLogService)
    {
        _serviceProvider = serviceProvider;
        _apiLogService = apiLogService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            //todo 測試 每分鐘更新一次,
            // 正式 每天更新一次
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            Console.WriteLine("ApiLogBackgroundService 正在執行");
            var logDic = _apiLogService.GetAndClearLogs();
            if (logDic.Count != 0)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<UserListDbContext>();
                    var logList = logDic
                        .Select(item => new ApiLog { UserId = item.Key, LogTime = item.Value })
                        .ToList();
                    dbContext.ApiLog.AddRange(logList);

                    await dbContext.SaveChangesAsync(stoppingToken);
                }
            }
        }
    }
}
