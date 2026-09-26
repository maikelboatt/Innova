using Innova.Application.Abstractions.Messaging;
using Innova.Application.GuestManagement.Guest.DTO;

namespace Innova.Application.GuestManagement.Guest.Queries
{
    public sealed record SearchGuestsQuery(
        string? NameContains,
        string? PhoneContains,
        bool? IsActive ):IQuery<IReadOnlyCollection<GuestSummaryDto>>;
}
