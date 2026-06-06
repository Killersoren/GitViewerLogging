using GitViewerLogging.DataAccess.Models;

namespace GitViewerLogging.Services
{
    public interface ILoggingQueryService
    {
        Task<IEnumerable<LogEntity>> GetRepoVisitorLogsAsync(Guid userId);
        Task <IEnumerable<LogEntity>> GetShareLinkVisitorLogsAsync(Guid userId, Guid shareLinkId);
        Task <IEnumerable<LogEntity>> GetUserShareLinkVisitorLogsAsync(Guid userId, Guid shareLinkId);
        Task<IEnumerable<LogEntity>> GetUserVisitorLogsAsync(Guid userId);
    }
}
