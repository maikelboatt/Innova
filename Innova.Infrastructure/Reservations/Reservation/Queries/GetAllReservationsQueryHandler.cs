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
    public sealed class GetAllReservationsQueryHandler( IDbConnectionProvider connectionProvider )
        :IQueryHandler<GetAllReservationsQuery, IReadOnlyCollection<ReservationDto>>
    {
        private static readonly Assembly Assembly = typeof(GetAllReservationsQueryHandler).Assembly;

        public async Task<IReadOnlyCollection<ReservationDto>> HandleAsync( GetAllReservationsQuery query, CancellationToken cancellationToken = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.Reservations.Reservation.Sql.GetAll.sql");

            CommandDefinition command = new(
                sql,
                transaction: connectionProvider.Transaction,
                cancellationToken: cancellationToken);

            IEnumerable<ReservationRow> rows = await connectionProvider.Connection.QueryAsync<ReservationRow>(command);

            return ReservationQueryHandlerMapper.MapToDto(rows);
        }
    }
}
