namespace Innova.Domain.Common
{
    public interface IAuditableEvent
    {
        Guid EntityId { get; }
        string EntityType { get; }
        string Summary { get; }
    }
}
