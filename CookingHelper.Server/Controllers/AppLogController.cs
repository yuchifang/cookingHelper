using CookingHelper.Data;
using CookingHelper.Model;
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
    public ActionResult GetLogList(string startTime, string endTime, string dateUnit)
    {
        var startTimeLong = Convert.ToInt64(startTime);
        var endTimeLong = Convert.ToInt64(endTime);

        var startTimeOffset = DateTimeOffset.FromUnixTimeSeconds(startTimeLong);
        var startDateTime = startTimeOffset.DateTime;
        var startDateOnly = DateOnly.FromDateTime(startDateTime);

        var endTimeOffset = DateTimeOffset.FromUnixTimeSeconds(endTimeLong);
        var endDateTime = endTimeOffset.DateTime;
        var endDateOnly = DateOnly.FromDateTime(endDateTime);

        List<ApiLog>? dateRangeLog = _userListDbContext
            .ApiLog.AsEnumerable()
            .Where(item => CustomCondition(item, endDateOnly, startDateOnly))
            .ToList();

        if (dateUnit == "day")
        {
            // 統計 ApiLog的資料,並去除重複的
            var noRepeatLogGroup = dateRangeLog
                .Select(entry => new
                { //! 轉 UTC +8
                    Date = DateOnly.FromDateTime(
                        DateTimeOffset.FromUnixTimeSeconds(entry.LogTime).LocalDateTime.Date
                    ),
                    entry.UserId
                })
                .Distinct()
                .GroupBy(entry => entry.Date)
                .Select(group => new ChatBar { Date = group.Key, Count = group.Count() });

            var DateUnit = 60 * 60 * 24;
            if ((endTimeLong - startTimeLong) / DateUnit > 100)
            {
                var logGroupData = noRepeatLogGroup.OrderBy(entry => entry.Date).ToList();

                var limitCount = 100;
                var offset = (endDateTime - startDateTime) / limitCount;

                // 依時間單位,時間範圍產生所有的空資料, 有值則填值
                var BarChartAdjustData = Enumerable
                    .Range(1, limitCount)
                    .Select((spaceCount) => startDateTime.Add(offset * spaceCount))
                    //! UTC+8
                    .Select(date => DateOnly.FromDateTime(date.ToLocalTime()))
                    .Select(date =>
                    {
                        var tempCount = 0;
                        var itemsToRemove = new List<ChatBar>();
                        foreach (var barChartItem in logGroupData)
                        {
                            if (barChartItem.Date <= date)
                            {
                                tempCount += barChartItem.Count;
                                itemsToRemove.Add(barChartItem);
                            }
                        }
                        foreach (var item in itemsToRemove)
                        {
                            logGroupData.Remove(item);
                        }
                        return new { Date = date.ToString("yyyy-MM-dd"), Count = tempCount };
                    })
                    .OrderBy(entry => entry.Date)
                    .ToList();

                return Ok(new { BarChartData = BarChartAdjustData, OverLimit = true });
            }

            var logGroupDic = noRepeatLogGroup.ToDictionary(
                entry => entry.Date,
                entry => entry.Count
            );

            var BarChartData = Enumerable
                .Range(0, (endDateTime - startDateTime).Days + 1)
                .Select(offset => startDateTime.AddDays(offset))
                //! UTC+8
                .Select(date => DateOnly.FromDateTime(date.ToLocalTime()))
                .Select(date =>
                {
                    return new
                    {
                        Date = date.ToString("MM-dd"),
                        Count = logGroupDic.ContainsKey(date) ? logGroupDic[date] : 0
                    };
                })
                .OrderBy(entry => entry.Date)
                .ToList();

            return Ok(new { BarChartData, OverLimit = false });
        }
        else if (dateUnit == "month")
        {
            var noRepeatLogGroup = dateRangeLog
                .Select(entry => new
                { //! 轉 UTC +8
                    Date = DateOnly.ParseExact(
                        DateTimeOffset
                            .FromUnixTimeSeconds(entry.LogTime)
                            .LocalDateTime.ToString("yyyy-MM"),
                        "yyyy-MM"
                    ),
                    entry.UserId
                })
                .Distinct()
                .GroupBy(entry => entry.Date)
                .Select(group => new ChatBar { Date = group.Key, Count = group.Count() });

            var totalMonth =
                ((endDateTime.Year - startDateTime.Year) * 12)
                + (endDateTime.Month - startDateTime.Month);

            if (totalMonth > 100)
            {
                var logData = noRepeatLogGroup.OrderBy(entry => entry.Date).ToList();

                var limitCount = 100;
                var offset = (endDateTime - startDateTime) / limitCount;

                var BarChartAdjustData = Enumerable
                    .Range(1, limitCount)
                    .Select((spaceCount) => startDateTime.Add(offset * spaceCount))
                    //! UTC+8
                    .Select(date =>
                        DateOnly.ParseExact(date.ToLocalTime().ToString("yyyy-MM"), "yyyy-MM")
                    )
                    .Select(date =>
                    {
                        var tempCount = 0;
                        var itemsToRemove = new List<ChatBar>();
                        foreach (var barChartItem in logData)
                        {
                            if (barChartItem.Date <= date)
                            {
                                tempCount += barChartItem.Count;
                                itemsToRemove.Add(barChartItem);
                            }
                        }
                        foreach (var item in itemsToRemove)
                        {
                            logData.Remove(item);
                        }
                        return new { Date = date.ToString("yyyy-MM"), Count = tempCount };
                    })
                    .OrderBy(entry => entry.Date)
                    .ToList();
                return Ok(new { BarChartData = BarChartAdjustData, OverLimit = true });
            }

            var logGroupDic = noRepeatLogGroup.ToDictionary(
                entry => entry.Date,
                entry => entry.Count
            );

            var BarChartData = Enumerable
                .Range(
                    0,
                    endDateTime.Month
                        - startDateTime.Month
                        + 12 * (endDateTime.Year - startDateTime.Year)
                        + 1
                )
                .Select(startDateTime.AddMonths)
                //! UTC+8
                .Select(date =>
                    DateOnly.ParseExact(date.ToLocalTime().ToString("yyyy-MM"), "yyyy-MM")
                )
                .Select(date => new
                {
                    Date = date.ToString("yyyy-MM"),
                    Count = logGroupDic.ContainsKey(date) ? logGroupDic[date] : 0
                })
                .OrderBy(entry => entry.Date)
                .ToList();
            return Ok(new { BarChartData, OverLimit = false });
        }
        else if (dateUnit == "year")
        {
            var noRepeatLogGroup = dateRangeLog
                .Select(entry => new
                { //! 轉 UTC +8
                    Date = DateOnly.ParseExact(
                        DateTimeOffset
                            .FromUnixTimeSeconds(entry.LogTime)
                            .LocalDateTime.ToString("yyyy"),
                        "yyyy"
                    ),
                    entry.UserId
                })
                .Distinct()
                .GroupBy(entry => entry.Date)
                .Select(group => new ChatBar { Date = group.Key, Count = group.Count() });

            var totalYear = endDateTime.Year - startDateTime.Year;
            if (totalYear > 100)
            {
                var logData = noRepeatLogGroup.OrderBy(entry => entry.Date).ToList();

                var limitCount = 100;
                var offset = (endDateTime - startDateTime) / limitCount;

                var BarChartAdjustData = Enumerable
                    .Range(1, limitCount)
                    .Select((spaceCount) => startDateTime.Add(offset * spaceCount))
                    //! UTC+8
                    .Select(date =>
                        DateOnly.ParseExact(date.ToLocalTime().ToString("yyyy"), "yyyy")
                    )
                    .Select(date =>
                    {
                        var tempCount = 0;
                        var itemsToRemove = new List<ChatBar>();
                        foreach (var barChartItem in logData)
                        {
                            if (barChartItem.Date <= date)
                            {
                                tempCount += barChartItem.Count;
                                itemsToRemove.Add(barChartItem);
                            }
                        }
                        foreach (var item in itemsToRemove)
                        {
                            logData.Remove(item);
                        }
                        return new { Date = date.ToString("yyyy"), Count = tempCount };
                    })
                    .OrderBy(entry => entry.Date)
                    .ToList();
                return Ok(new { BarChartData = BarChartAdjustData, OverLimit = true });
            }

            var logGroupDic = noRepeatLogGroup.ToDictionary(
                entry => entry.Date,
                entry => entry.Count
            );

            var BarChartData = Enumerable
                .Range(0, endDateTime.Year - startDateTime.Year + 1)
                .Select(startDateTime.AddYears)
                //! UTC+8
                .Select(date => DateOnly.ParseExact(date.ToLocalTime().ToString("yyyy"), "yyyy"))
                .Select(date => new
                {
                    Date = date.ToString("yyyy"),
                    Count = logGroupDic.ContainsKey(date) ? logGroupDic[date] : 0
                })
                .OrderBy(entry => entry.Date)
                .ToList();
            return Ok(new { BarChartData, OverLimit = false });
        }
        return NotFound();
    }

    static bool CustomCondition(ApiLog ApiLog, DateOnly endDateOnly, DateOnly startDateOnly)
    {
        return endDateOnly
                >= DateOnly.FromDateTime(
                    DateTimeOffset.FromUnixTimeSeconds(ApiLog.LogTime).DateTime
                )
            && DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(ApiLog.LogTime).DateTime)
                >= startDateOnly;
    }

    public class ChatBar
    {
        public DateOnly Date { get; set; } = default!;
        public int Count { get; set; } = default!;
    }
}
