using Innova.Domain.Common;

namespace Innova.Domain.GuestManagement.Events
{
    public sealed record GuestUpdated( Guid GuestId, string PhoneNumber, string? Email ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTime.UtcNow;
    }
}
