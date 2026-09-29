namespace Innova.Application.Shared.Abstractions
{
    // Writes a structured audit trail — who did what, when.
    // Implementation writes to a dedicated audit table in the DB.
    public interface IAuditLogger
    {
        Task LogAsync(
            string eventName,
            string entityType,
            Guid entityId,
            string summary,
            CancellationToken ct = default );
    }
}
