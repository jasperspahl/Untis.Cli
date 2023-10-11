using Untis.Client.Contracts;
using Untis.Client.Contracts.Enum;

namespace Untis.Client;

public interface IUntisClient: IDisposable
{
    Task<bool> LoginAsync();
    Task<bool> LogoutAsync();
    Task<bool> IsLoggedInAsync();
    Task<IEnumerable<Lesson>> GetOwnTimetableForTodayAsync();
    Task<IEnumerable<Lesson>> GetOwnTimetableForThisWeekAsync();
    Task<IEnumerable<Lesson>> GetOwnTimetableFor(DateTime date);
    Task<IEnumerable<Lesson>> GetOwnTimetableAsync(DateTime? start, DateTime? end);
    Task<IEnumerable<Lesson>> GetTimetableAsync(int id, UntisElementType type, DateTime? start, DateTime? end);
}