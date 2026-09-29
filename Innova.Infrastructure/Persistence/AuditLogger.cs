// Innova.Infrastructure/Persistence/AuditLogger.cs

using Dapper;
using Innova.Application.Abstractions.Services;
using Innova.Application.Shared.Abstractions;
using Innova.Infrastructure.Persistence.Connections;
using Microsoft.Extensions.Logging;

namespace Innova.Infrastructure.Persistence
{
    public sealed class AuditLogger(
        IDbConnectionProvider connectionProvider,
        ICurrentUserContext currentUserContext,
        ILogger<AuditLogger> logger )
        :IAuditLogger
    {
        public async Task LogAsync(
            string eventName,
            string entityType,
            Guid entityId,
            string summary,
            CancellationToken ct = default )
        {
            const string sql = """
                               INSERT INTO audit.AuditLog
                                   (EventName, EntityType, EntityId, Summary,Username, OccurredAt)
                               VALUES
                                   (@EventName, @EntityType, @EntityId, @Summary,@Username, SYSUTCDATETIME())
                               """;

            try
            {
                await connectionProvider.Connection.ExecuteAsync(
                    new CommandDefinition(
                        sql,
                        new
                        {
                            EventName = eventName,
                            EntityType = entityType,
                            EntityId = entityId,
                            Summary = summary,
                            currentUserContext.Username
                        },
                        connectionProvider.Transaction,
                        cancellationToken: ct));
            }
            catch (Exception ex)
            {
                // Audit failure must never crash the main operation.
                // Log to Serilog as a fallback — the audit trail has
                // a gap but the business operation is preserved.
                logger.LogError(
                    ex,
                    "Audit log write failed for event {EventName} on {EntityType} {EntityId}. " +
                    "Summary: {Summary}",
                    eventName,
                    entityType,
                    entityId,
                    summary);
            }
        }
    }
}
