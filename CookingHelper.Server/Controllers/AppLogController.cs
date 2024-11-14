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
    //! UTC +0 Time
    public async Task<ActionResult> GetLogList(string startTime, string endTime, string dateUnit)
    {
        // dateUnit 日 月 年
        long unit;
        bool isExceedLimit = false; // 超過 100 筆計算方式會改變
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

        // 超過 100 筆計算方式會改變
        if (dateUnit == "day")
        {
            unit = 60 * 60 * 24;
            if ((endTimeLong - startTimeLong) / unit > 100)
            {
                isExceedLimit = true;
            }
        }
        else if (dateUnit == "month")
        {
            var totalMonth =
                ((endDateTime.Year - startDateTime.Year) * 12)
                + (endDateTime.Month - startDateTime.Month);
            if (totalMonth > 100)
            {
                isExceedLimit = true;
            }
        }
        else if (dateUnit == "year")
        {
            var totalYear = endDateTime.Year - startDateTime.Year;
            if (totalYear > 100)
            {
                isExceedLimit = true;
            }
        }

        if (!isExceedLimit)
        {
            if (dateUnit == "day")
            {
                var BarChartDic = logList
                    .Select(entry => new
                    { //! 轉 UTC +8
                        Date = DateTimeOffset.FromUnixTimeSeconds(entry.LogTime).LocalDateTime.Date,
                        entry.UserId
                    })
                    .Distinct()
                    .GroupBy(entry => entry.Date)
                    .Select(group => new
                    {
                        Date = DateOnly.FromDateTime(group.Key).ToString("yyyy-MM-dd"),
                        Count = group.Count()
                    })
                    .ToDictionary(entry => entry.Date, entry => entry.Count);

                var BarChartData = Enumerable
                    .Range(0, (endDateTime - startDateTime).Days + 1)
                    .Select(offset => startDateTime.AddDays(offset))
                    //! UTC+8
                    .Select(date =>
                        DateOnly.FromDateTime(date.ToLocalTime()).ToString("yyyy-MM-dd")
                    )
                    .Select(date => new
                    {
                        Date = date,
                        Count = BarChartDic.ContainsKey(date) ? BarChartDic[date] : 0
                    })
                    .OrderBy(entry => entry.Date)
                    .ToList();

                return Ok(BarChartData);
            }
        }
        else { } // 建立超過 limit 沒超過 limit 的算法
        return Ok();
    }
}

/*
 
        var students = new List<Student>
        {
            new Student
            {
                Name = "Alice",
                Grade = "A",
                Score = 85
            },
            new Student
            {
                Name = "Bob",
                Grade = "B",
                Score = 75
            },
            new Student
            {
                Name = "Charlie",
                Grade = "A",
                Score = 95
            },
            new Student
            {
                Name = "David",
                Grade = "B",
                Score = 80
            },
            new Student
            {
                Name = "Eve",
                Grade = "C",
                Score = 70
            }
        };

        // 使用 GroupBy 按年級分組
        var groupedStudents = students.GroupBy(student => student.Grade);

        // 輸出分組結果和平均分數
        foreach (var group in groupedStudents)
        {
            Console.WriteLine($"年級: {group.Key}, 平均分數: {group.Count()}");
        }
        public class Student
    {
        public string Name { get; set; }
        public string Grade { get; set; }
        public int Score { get; set; }
    }*/
