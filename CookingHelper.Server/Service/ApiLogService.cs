namespace CookingHelper.Service;

public class ApiLogService
{
    private readonly Dictionary<string, DateTime> _logs = new Dictionary<string, DateTime>();

    public void AddLog(string userId, DateTime time)
    {
        lock (_logs)
        {
            _logs.Add(userId, time);
        }
    }

    public Dictionary<string, DateTime> GetLogs()
    {
        return _logs;
    }

    public Dictionary<string, DateTime> GetAndClearLogs()
    {
        lock (_logs)
        {
            var logsToSave = new Dictionary<string, DateTime>(_logs);
            _logs.Clear();
            return logsToSave;
        }
    }
}
