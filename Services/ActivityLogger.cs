using Staybnb.Data;
using Staybnb.Models;
namespace Staybnb.Services;
public interface IActivityLogger { Task LogAsync(string? userId, string action, string details); }
public class ActivityLogger : IActivityLogger
{
    private readonly ApplicationDbContext _db;
    public ActivityLogger(ApplicationDbContext db) => _db = db;
    public async Task LogAsync(string? userId, string action, string details)
    {
        _db.ActivityLogs.Add(new ActivityLog { UserId = userId, Action = action, Details = details });
        await _db.SaveChangesAsync();
    }
}
