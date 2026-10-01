using System.Reflection;
using Dapper;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.GuestManagement.Guest.DTO;
using Innova.Application.GuestManagement.Guest.Queries;
using Innova.Infrastructure.Persistence;
using Innova.Infrastructure.Persistence.Connections;

namespace Innova.Infrastructure.GuestManagement.Guest.Queries
{
    public sealed class SearchGuestsQueryHandler(
        IDbConnectionProvider connectionProvider )
        :IQueryHandler<SearchGuestsQuery, IReadOnlyCollection<GuestSummaryDto>>
    {
        private static readonly Assembly Assembly =
            typeof(SearchGuestsQueryHandler).Assembly;

        public async Task<IReadOnlyCollection<GuestSummaryDto>> HandleAsync(
            SearchGuestsQuery query,
            CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(
                Assembly,
                "Innova.Infrastructure.GuestManagement.Guest.Sql.SearchQuery.sql");

            IEnumerable<GuestSummaryDto> guests =
                await connectionProvider.Connection.QueryAsync<GuestSummaryDto>(
                    new CommandDefinition(
                        sql,
                        new
                        {
                            query.NameContains,
                            query.PhoneContains,
                            query.IsActive
                        },
                        connectionProvider.Transaction,
                        cancellationToken: ct));

            return guests.ToList();
        }
    }
}
