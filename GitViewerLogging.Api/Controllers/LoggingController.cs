using GitViewerLogging.DataAccess.Models;
using GitViewerLogging.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GitViewerLogging.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoggingController : ControllerBase
    {
        private readonly ILoggingQueryService _queryService;

        public LoggingController(ILoggingQueryService queryService) => _queryService = queryService;

        [Authorize(Roles = "User,Admin")]
        [HttpGet("get-repo-visitor-logs")]
        public async Task<ActionResult<IEnumerable<LogEntity>>> GetRepoVisitorLogs()
        {
            var userId = GetUserId();
            if (userId is null)
            {
                return Unauthorized();
            }

            var logs = await _queryService.GetRepoVisitorLogsAsync(userId.Value);
            return Ok(logs);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("get-user-visitor-logs")]
        public async Task<ActionResult<IEnumerable<LogEntity>>> GetUserVisitorLogs()
        {
            var userId = GetUserId();
            if (userId is null)
            {
                return Unauthorized();
            }

            var logs = await _queryService.GetUserVisitorLogsAsync(userId.Value);
            return Ok(logs);
        }

        private Guid? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim is null ? null : Guid.Parse(claim.Value);
        }
    }
}