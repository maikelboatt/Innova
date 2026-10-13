using Innova.Application.Abstractions.Messaging;
using Innova.Application.Billing.Folio.DTO;

namespace Innova.Application.Billing.Folio.Queries
{
    /// <summary>
    ///     What a folio of the given owner type can be opened for: active guests (Guest), occupants of in-house stays
    ///     (GuestWithinStay), in-house stays (Stay) or group bookings (GroupBooking).
    /// </summary>
    public sealed record ListFolioOwnerOptionsQuery( string OwnerType ):IQuery<IReadOnlyCollection<FolioOwnerOptionDto>>;
}
