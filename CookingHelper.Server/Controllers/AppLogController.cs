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
    public async Task<ActionResult> GetLogList(DateTime startTime, DateTime endTime)
    {
        var logList = await _userListDbContext
            .ApiLog.Where(item => startTime <= item.LogTime && item.LogTime <= endTime)
            .ToListAsync();

        return Ok(logList);
    }
}
