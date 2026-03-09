namespace GitViewerLogging.DataAccess.Models
{
    public class LogEntity
    {
        // Primary key
        public Guid Id { get; set; }

        // Event information
        public required string EventType { get; set; }
        public required string EntityType { get; set; }
        public string? EntityName { get; set; }
        public Guid? EntityId { get; set; }

        // User information
        public DateTime TimeStamp { get; set; }
        public Guid? UserId { get; set; }
        public bool? IsAnonymous { get; set; }

        // Other stuff
        public string? Details { get; set; }
    }
}
