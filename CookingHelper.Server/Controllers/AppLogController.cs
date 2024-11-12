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
    public async Task<ActionResult> GetLogList(string startTime, string endTime)
    {
        var startTimeLong = Convert.ToInt64(startTime);
        var endTimeLong = Convert.ToInt64(endTime);

        var logList = await _userListDbContext
            .ApiLog.Where(item => endTimeLong >= item.LogTime && item.LogTime >= startTimeLong)
            .ToListAsync();

        return Ok(logList);
    }
}
