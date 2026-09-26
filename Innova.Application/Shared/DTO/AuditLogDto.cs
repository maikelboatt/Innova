namespace Innova.Application.Shared.DTO
{
    public sealed record AuditLogDto(
        int Id,
        string EventName,
        string EntityType,
        Guid EntityId,
        string Summary,
        string UserName,
        DateTime OccurredAt );
}
