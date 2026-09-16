using Innova.Domain.Common;

namespace Innova.Domain.GuestManagement.Events
{
    public sealed record GuestReactivated( Guid GuestId ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTime.UtcNow;
    }
}
