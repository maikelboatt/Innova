-- GetRecentAuditLog.sql
SELECT TOP (@Take) Id, EventName, EntityType, EntityId, Summary, Username, OccurredAt
FROM audit.AuditLog
ORDER BY OccurredAt DESC;