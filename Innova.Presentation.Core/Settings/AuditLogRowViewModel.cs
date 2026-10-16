using Innova.Application.Shared.DTO;

namespace Innova.Presentation.Core.Settings
{
    public sealed class AuditLogRowViewModel( AuditLogDto dto )
    {
        public string EventName { get; } = dto.EventName;
        public string EntityType { get; } = dto.EntityType;
        public Guid EntityId { get; } = dto.EntityId;
        public string Summary { get; } = dto.Summary;
        public string UserName { get; } = dto.UserName;

        public string OccurredAtDisplay { get; } = dto.OccurredAt.ToString("dd MMM yyyy, h:mm tt");
    }
}
