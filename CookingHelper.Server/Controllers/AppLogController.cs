using CookingHelper.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AppLogController : ControllerBase
{
    private readonly UserListDbContext _userListDbContext;

    public AppLogController(UserListDbContext UserListDbContext)
    {
        _userListDbContext = UserListDbContext;
    }

    [HttpGet("getLogList")]
    //! UTC +0 Second Time
    public async Task<ActionResult> GetLogList(string startTime, string endTime, string dateUnit)
    {
        // dateUnit 日 月 年
        long unit;

        Console.WriteLine(startTime + "startTime");
        var startTimeLong = Convert.ToInt64(startTime);
        var endTimeLong = Convert.ToInt64(endTime);

        List<ApiLog>? logList = await _userListDbContext
            .ApiLog.Where(item => endTimeLong >= item.LogTime && item.LogTime >= startTimeLong)
            .ToListAsync();
        /*
            如果超過 100 筆計算方式會改變
            空值也算
            這段時間 除以單位 整數為多少 代表有幾筆資料
        */
        Console.WriteLine(startTimeLong + "startTimeLong");
        var startTimeOffset = DateTimeOffset.FromUnixTimeSeconds(startTimeLong);
        var startDateTime = startTimeOffset.DateTime;

        var endTimeOffset = DateTimeOffset.FromUnixTimeSeconds(endTimeLong);
        var endDateTime = endTimeOffset.DateTime;
        //! 簡化這部分

        //! 完成 year
        //! 建立 圖表的 page
        //! 建立 日歷的 page
        //! 建立 分頁的 page

        //! 加 alert
        //! 只要 string 前五個字
        if (dateUnit == "day")
        {
            // 統計 logList的資料,並去除重複的
            var BarChartList = logList
                .Select(entry => new
                { //! 轉 UTC +8
                    Date = DateTimeOffset.FromUnixTimeSeconds(entry.LogTime).LocalDateTime.Date,
                    entry.UserId
                })
                .Distinct()
                .GroupBy(entry => entry.Date)
                .Select(group => new ChatBar
                {
                    Date = DateOnly.FromDateTime(group.Key),
                    Count = group.Count()
                });

            unit = 60 * 60 * 24;
            if ((endTimeLong - startTimeLong) / unit > 100)
            {
                var BarChartListList = BarChartList.OrderBy(entry => entry.Date).ToList();
                var limitCount = 100;
                var random = new Random();
                var offset = (endDateTime - startDateTime) / limitCount;
                Console.WriteLine(offset + "offset");
                Console.WriteLine(startDateTime + "startDateTime");
                Console.WriteLine(endDateTime + "endDateTime");
                // 依時間單位,時間範圍產生 所有的空資料, 有值則填值
                var DateList = Enumerable
                    .Range(1, limitCount)
                    .Select((space) => startDateTime.Add(offset * space))
                    //! UTC+8
                    .Select(date => DateOnly.FromDateTime(date.ToLocalTime()))
                    .Select(date =>
                    {
                        var tempCount = 0;
                        var itemsToRemove = new List<ChatBar>();
                        foreach (ChatBar barChartItem in BarChartListList)
                        {
                            if (barChartItem.Date < date)
                            {
                                tempCount += barChartItem.Count;
                                itemsToRemove.Add(barChartItem);
                            }
                        }
                        foreach (var item in itemsToRemove)
                        {
                            BarChartListList.Remove(item);
                        }
                        return new { Date = date.ToString("yyyy-MM-dd"), Count = tempCount };
                    })
                    .OrderBy(entry => entry.Date)
                    .ToList();

                return Ok(DateList);
            }

            var BarChartDic = BarChartList.ToDictionary(entry => entry.Date, entry => entry.Count);
            var BarChartData = Enumerable
                .Range(0, (endDateTime - startDateTime).Days + 1)
                .Select(offset => startDateTime.AddDays(offset))
                //! UTC+8
                .Select(date => DateOnly.FromDateTime(date.ToLocalTime()))
                .Select(date => new
                {
                    Date = date.ToString("yyyy-MM-dd"),
                    Count = BarChartDic.ContainsKey(date) ? BarChartDic[date] : 0
                })
                .OrderBy(entry => entry.Date)
                .ToList();

            return Ok(BarChartData);
        }
        else if (dateUnit == "month")
        {
            var BarChartDic = logList
                .Select(entry => new
                { //! 轉 UTC +8
                    Date = DateTimeOffset
                        .FromUnixTimeSeconds(entry.LogTime)
                        .LocalDateTime.ToString("yyyy-MM"), // todo month
                    entry.UserId
                })
                .Distinct()
                .GroupBy(entry => entry.Date)
                .Select(group => new
                {
                    Date = group.Key, //todo month
                    Count = group.Count()
                })
                .ToDictionary(entry => entry.Date, entry => entry.Count);
            var totalMonth =
                ((endDateTime.Year - startDateTime.Year) * 12)
                + (endDateTime.Month - startDateTime.Month);

            if (totalMonth > 100)
            {
                var sampleCount = 100;
                var random = new Random();

                var DateList = Enumerable
                    .Range(
                        0,
                        endDateTime.Month
                            - startDateTime.Month
                            + 12 * (endDateTime.Year - startDateTime.Year)
                            + 1
                    )
                    .Select(startDateTime.AddMonths)
                    //! UTC+8
                    .Select(date => DateOnly.FromDateTime(date.ToLocalTime()).ToString("yyyy-MM"))
                    .OrderBy(x => random.Next())
                    .Take(sampleCount)
                    .Select(date => new
                    {
                        Date = date,
                        Count = BarChartDic.ContainsKey(date) ? BarChartDic[date] : 0
                    })
                    .OrderBy(entry => entry.Date)
                    .ToList();
                return Ok(DateList);
            }
            //! Timespan Page
            var BarChartData = Enumerable
                .Range(
                    0,
                    endDateTime.Month
                        - startDateTime.Month
                        + 12 * (endDateTime.Year - startDateTime.Year)
                        + 1
                ) //todo month
                .Select(startDateTime.AddMonths)
                //! UTC+8
                .Select(date => DateOnly.FromDateTime(date.ToLocalTime()).ToString("yyyy-MM"))
                .Select(date => new
                {
                    Date = date,
                    Count = BarChartDic.ContainsKey(date) ? BarChartDic[date] : 0
                })
                .OrderBy(entry => entry.Date)
                .ToList();
            return Ok(BarChartData);
        }
        else if (dateUnit == "year")
        {
            var BarChartDic = logList
                .Select(entry => new
                { //! 轉 UTC +8
                    Date = DateTimeOffset
                        .FromUnixTimeSeconds(entry.LogTime)
                        .LocalDateTime.ToString("yyyy"), // todo month
                    entry.UserId
                })
                .Distinct()
                .GroupBy(entry => entry.Date)
                .Select(group => new { Date = group.Key, Count = group.Count() })
                .ToDictionary(entry => entry.Date, entry => entry.Count);
            var totalYear = endDateTime.Year - startDateTime.Year;
            if (totalYear > 100)
            {
                var sampleCount = 100;
                var random = new Random();

                var DateList = Enumerable
                    .Range(0, endDateTime.Year - startDateTime.Year + 1)
                    .Select(startDateTime.AddYears)
                    //! UTC+8
                    .Select(date => DateOnly.FromDateTime(date.ToLocalTime()).ToString("yyyy"))
                    .OrderBy(x => random.Next())
                    .Take(sampleCount)
                    .Select(date => new
                    {
                        Date = date,
                        Count = BarChartDic.ContainsKey(date) ? BarChartDic[date] : 0
                    })
                    .OrderBy(entry => entry.Date)
                    .ToList();
                return Ok(DateList);
            }
            var BarChartData = Enumerable
                .Range(0, endDateTime.Year - startDateTime.Year + 1)
                .Select(startDateTime.AddYears)
                //! UTC+8
                .Select(date => DateOnly.FromDateTime(date.ToLocalTime()).ToString("yyyy"))
                .Select(date => new
                {
                    Date = date,
                    Count = BarChartDic.ContainsKey(date) ? BarChartDic[date] : 0
                })
                .OrderBy(entry => entry.Date)
                .ToList();
            return Ok(BarChartData);
        }
        return NotFound();
    }

    public class ChatBar
    {
        public DateOnly Date { get; set; } = default!;
        public int Count { get; set; } = default!;
    }
}
