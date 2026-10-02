using System.Reflection;
using Dapper;
using Innova.Domain.Reservations.Repositories;
using Innova.Domain.Reservations.ValueObjects;
using Innova.Infrastructure.Persistence;
using Innova.Infrastructure.Persistence.Connections;
using Innova.Infrastructure.Reservations.Reservation.Mapping;

namespace Innova.Infrastructure.Reservations.Reservation.Repository
{
    public sealed class ReservationRepository( IDbConnectionProvider connectionProvider ):IReservationRepository
    {
        private static readonly Assembly Assembly = typeof(ReservationRepository).Assembly;

        public async Task<Domain.Reservations.Aggregates.Reservation?> GetByIdAsync( ReservationId reservationId, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.Reservations.Reservation.Sql.GetById.sql");

            ReservationRow? row = await connectionProvider.Connection.QuerySingleOrDefaultAsync<ReservationRow>(
                                      new CommandDefinition(
                                          sql,
                                          new
                                          {
                                              ReservationId = reservationId.Value
                                          },
                                          connectionProvider.Transaction,
                                          cancellationToken: ct));

            return row is null
                       ? null
                       : ReservationMapper.ToDomain(row);
        }

        public async Task SaveAsync( Domain.Reservations.Aggregates.Reservation reservation, CancellationToken ct = default )
        {
            ReservationRow model = ReservationMapper.ToPersistenceModel(reservation);

            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.Reservations.Reservation.Sql.Insert.sql");

            CommandDefinition cmd = new(
                sql,
                model,
                connectionProvider.Transaction,
                cancellationToken: ct);

            await connectionProvider.Connection.ExecuteAsync(cmd);
        }

        public async Task UpdateAsync( Domain.Reservations.Aggregates.Reservation reservation, CancellationToken ct = default )
        {
            ReservationRow model = ReservationMapper.ToPersistenceModel(reservation);

            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.Reservations.Reservation.Sql.Update.sql");

            CommandDefinition cmd = new(
                sql,
                model,
                connectionProvider.Transaction,
                cancellationToken: ct);

            await connectionProvider.Connection.ExecuteAsync(cmd);
        }
    }
}
