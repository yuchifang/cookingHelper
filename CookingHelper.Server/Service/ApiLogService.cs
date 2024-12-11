namespace CookingHelper.Service;

public class ApiLogService
{
    private readonly List<Log> _logs = new List<Log>();

    public void AddLog(string userId, long time)
    {
        lock (_logs)
        {
            _logs.Add(new Log { UserId = userId, Time = time });
        }
    }

    public List<Log> GetAndClearLogs()
    {
        lock (_logs)
        {
            var logsToSave = new List<Log>(_logs);
            _logs.Clear();
            return logsToSave;
        }
    }

    public class Log
    {
        public string UserId { get; set; } = default!;
        public long Time { get; set; } = default!;
    }
}
