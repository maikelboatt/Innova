using Innova.Domain.Common;

namespace Innova.Domain.Reservations.Events
{
    public sealed record GroupBookingOpened( Guid GroupBookingId, Guid OrganizerGuestId, string GroupName ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
