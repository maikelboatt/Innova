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
    public sealed class GetReservationByIdQueryHandler( IDbConnectionProvider connectionProvider ):IQueryHandler<GetReservationByIdQuery, ReservationDto?>
    {
        private static readonly Assembly Assembly = typeof(GetReservationByIdQueryHandler).Assembly;

        public async Task<ReservationDto?> HandleAsync( GetReservationByIdQuery query, CancellationToken cancellationToken = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.Reservations.Reservation.Sql.GetById.sql");

            CommandDefinition command = new(
                sql,
                new
                {
                    query.ReservationId
                },
                connectionProvider.Transaction,
                cancellationToken: cancellationToken);

            ReservationRow? row =
                await connectionProvider.Connection
                                        .QuerySingleOrDefaultAsync<ReservationRow>(command);

            return row is null
                       ? null
                       : ReservationQueryHandlerMapper.ToDto(row);
        }
    }
}
