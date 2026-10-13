namespace Innova.Application.Billing.Folio.DTO
{
    /// <summary>
    ///     Lookup row for the Open Folio form: something a folio can be opened for. Label/Detail are display text.
    ///     OpenFolioCount lets the form warn that the owner already has an open folio.
    /// </summary>
    public sealed record FolioOwnerOptionDto(
        Guid OwnerId,
        string Label,
        string? Detail,
        int OpenFolioCount );
}
