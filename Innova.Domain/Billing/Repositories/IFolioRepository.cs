using Innova.Domain.Billing.Aggregates;
using Innova.Domain.Billing.ValueObjects;

namespace Innova.Domain.Billing.Repositories
{
    public interface IFolioRepository
    {
        Task<Folio?> GetByIdAsync( FolioId id, CancellationToken ct = default );

        // The checkout balance check needs every folio tied to a stay: one
        // owned directly by the StayId, one per occupant GuestId for
        // split billing, and one for the GroupBookingId if set.
        // CheckOutCommandHandler calls this once per owner-type/id pair it
        // has on hand and sums the results before allowing Stay.CheckOut().
        Task<IReadOnlyCollection<Folio>> GetByOwnerAsync(
            FolioOwnerType ownerType,
            Guid ownerId,
            CancellationToken ct = default );

        Task SaveAsync( Folio folio, CancellationToken ct = default );

        Task UpdateAsync( Folio folio, CancellationToken ct = default );
    }
}
