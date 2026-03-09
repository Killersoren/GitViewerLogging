using GitViewerLogging.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace GitViewerLogging.Services
{
    public class LoggingQueryService : ILoggingQueryService
    {
        private readonly LoggingContext _context;
        private const int LookbackDays = 30;

        public LoggingQueryService(LoggingContext context) => _context = context;

        public async Task<IEnumerable<LogEntity>> GetRepoVisitorLogsAsync(Guid userId) =>
            await _context.LogEntities
                .Where(log =>
                    (log.IsAnonymous ?? false) &&
                    log.Details == $"RepositoryOwner:{userId}" &&
                    log.EventType == "RepositoryViewed" &&
                    log.TimeStamp >= DateTime.UtcNow.AddDays(-LookbackDays)
                )
                .OrderByDescending(log => log.TimeStamp)
                .ToListAsync();

        public async Task<IEnumerable<LogEntity>> GetUserVisitorLogsAsync(Guid userId) =>
            await _context.LogEntities
                .Where(log =>
                    (log.IsAnonymous ?? false) &&
                    log.Details == $"PublicReposOwner:{userId}" &&
                    log.EventType == "PublicReposViewed" &&
                    log.TimeStamp >= DateTime.UtcNow.AddDays(-LookbackDays)
                )
                .OrderByDescending(log => log.TimeStamp)
                .ToListAsync();
    }
}