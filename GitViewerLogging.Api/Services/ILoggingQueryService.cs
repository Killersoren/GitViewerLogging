using GitViewerLogging.DataAccess.Models;

namespace GitViewerLogging.Services
{
    public interface ILoggingQueryService
    {
        Task<IEnumerable<LogEntity>> GetRepoVisitorLogsAsync(Guid userId);
        Task<IEnumerable<LogEntity>> GetUserVisitorLogsAsync(Guid userId);
    }
}
