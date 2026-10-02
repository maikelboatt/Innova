using System.Reflection;
using Dapper;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Reservations.DTO;
using Innova.Application.Reservations.Queries;
using Innova.Infrastructure.Persistence;
using Innova.Infrastructure.Persistence.Connections;
using Innova.Infrastructure.Reservations.Reservation.Mapping;

namespace Innova.Infrastructure.Reservations.Reservation.Queries
{
    public sealed class ListReservationsQueryHandler( IDbConnectionProvider connectionProvider )
        :IQueryHandler<ListReservationsQuery, IReadOnlyCollection<ReservationSummaryDto>>
    {
        private static readonly Assembly Assembly = typeof(ListReservationsQueryHandler).Assembly;

        public async Task<IReadOnlyCollection<ReservationSummaryDto>> HandleAsync( ListReservationsQuery query, CancellationToken cancellationToken = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.Reservations.Reservation.Sql.List.sql");

            CommandDefinition command = new(
                sql,
                new
                {
                    query.GuestId,
                    query.Status,
                    query.ArrivingOn,
                    query.DepartingOn
                },
                connectionProvider.Transaction,
                cancellationToken: cancellationToken);

            IEnumerable<ReservationSummaryRow> rows = await connectionProvider.Connection.QueryAsync<ReservationSummaryRow>(command);

            return ReservationQueryHandlerMapper.MapToSummaryDto(rows);
        }
    }
}
