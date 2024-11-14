namespace CookingHelper.Service;

public class ApiLogService
{
    private readonly Dictionary<string, long> _logs = new Dictionary<string, long>();

    public void AddLog(string userId, long time)
    {
        lock (_logs)
        {
            _logs.Add(userId, time);
        }
    }

    public Dictionary<string, long> GetAndClearLogs()
    {
        lock (_logs)
        {
            var logsToSave = new Dictionary<string, long>(_logs);
            _logs.Clear();
            return logsToSave;
        }
    }
}
