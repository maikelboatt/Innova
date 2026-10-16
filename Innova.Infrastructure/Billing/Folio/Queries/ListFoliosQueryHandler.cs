using System.Reflection;
using Dapper;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Billing.Folio.DTO;
using Innova.Application.Billing.Folio.Queries;
using Innova.Infrastructure.Persistence;
using Innova.Infrastructure.Persistence.Connections;

namespace Innova.Infrastructure.Billing.Folio.Queries
{
    public sealed class ListFoliosQueryHandler( IDbConnectionProvider connectionProvider )
        :IQueryHandler<ListFoliosQuery, IReadOnlyCollection<FolioListItemDto>>
    {
        private static readonly Assembly Assembly = typeof(ListFoliosQueryHandler).Assembly;

        public async Task<IReadOnlyCollection<FolioListItemDto>> HandleAsync( ListFoliosQuery query, CancellationToken cancellationToken = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.Billing.Folio.Sql.List.sql");

            CommandDefinition command = new(
                sql,
                new
                {
                    query.Status,
                    FolioId = (Guid?)null
                },
                connectionProvider.Transaction,
                cancellationToken: cancellationToken);

            IEnumerable<FolioListItemRow> rows = await connectionProvider.Connection.QueryAsync<FolioListItemRow>(command);

            return [.. rows.Select(FolioListItemRow.ToDto)];
        }
    }

    internal sealed class FolioListItemRow
    {
        public Guid FolioId { get; set; }
        public string OwnerType { get; set; } = string.Empty;
        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public string? OwnerDetail { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalCharges { get; set; }
        public decimal TotalPayments { get; set; }
        public decimal TotalRefunds { get; set; }
        public decimal TotalAdjustments { get; set; }
        public decimal Balance { get; set; }
        public DateTime? LastActivityAt { get; set; }

        public static FolioListItemDto ToDto( FolioListItemRow row ) => new(
            row.FolioId,
            row.OwnerType,
            row.OwnerId,
            row.OwnerName,
            row.OwnerDetail,
            row.Currency,
            row.Status,
            row.TotalCharges,
            row.TotalPayments,
            row.TotalRefunds,
            row.TotalAdjustments,
            row.Balance,
            row.LastActivityAt);
    }
}
