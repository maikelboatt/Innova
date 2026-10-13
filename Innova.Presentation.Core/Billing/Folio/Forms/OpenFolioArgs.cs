using Innova.Domain.Billing.ValueObjects;

namespace Innova.Presentation.Core.Billing.Folio.Forms
{
    /// <summary>
    ///     Optional pre-fill for the Open Folio form, e.g. when it is opened from a stay or a guest screen
    ///     so the owner is already chosen.
    /// </summary>
    public sealed record OpenFolioArgs( FolioOwnerType? OwnerType = null, Guid? OwnerId = null );
}
