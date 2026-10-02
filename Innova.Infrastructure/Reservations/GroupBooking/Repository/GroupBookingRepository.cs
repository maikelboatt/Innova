using System.Reflection;
using Dapper;
using Innova.Domain.Reservations.Repositories;
using Innova.Domain.Reservations.ValueObjects;
using Innova.Infrastructure.Persistence;
using Innova.Infrastructure.Persistence.Connections;
using Innova.Infrastructure.Reservations.GroupBooking.Mapping;

namespace Innova.Infrastructure.Reservations.GroupBooking.Repository
{
    public sealed class GroupBookingRepository( IDbConnectionProvider connectionProvider ):IGroupBookingRepository
    {
        private static readonly Assembly Assembly = typeof(GroupBookingRepository).Assembly;

        public async Task<Domain.Reservations.Aggregates.GroupBooking?> GetByIdAsync( GroupBookingId id, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.Reservations.GroupBooking.Sql.GetById.sql");

            CommandDefinition command = new(
                sql,
                new
                {
                    GroupBookingId = id.Value
                },
                connectionProvider.Transaction,
                cancellationToken: ct);


            using SqlMapper.GridReader multi =
                await connectionProvider.Connection.QueryMultipleAsync(command);

            GroupBookingRow? groupRow =
                await multi.ReadSingleOrDefaultAsync<GroupBookingRow>();

            if (groupRow is null)
                return null;

            IEnumerable<GroupBookingReservationRow> reservationRows =
                await multi.ReadAsync<GroupBookingReservationRow>();

            GroupBookingRow completeRow = new()
                                          {
                                              Id = groupRow.Id,
                                              OrganizerGuestId = groupRow.OrganizerGuestId,
                                              GroupName = groupRow.GroupName,
                                              ReservationIds = reservationRows
                                                               .Select(row => row.ReservationId)
                                                               .ToList()
                                                               .AsReadOnly()
                                          };

            return GroupBookingMapper.ToDomain(completeRow);
        }

        public async Task SaveAsync( Domain.Reservations.Aggregates.GroupBooking groupBooking, CancellationToken ct = default )
        {
            GroupBookingRow model = GroupBookingMapper.ToPersistenceModel(groupBooking);

            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.Reservations.GroupBooking.Sql.Insert.sql");

            CommandDefinition cmd = new(
                sql,
                model,
                connectionProvider.Transaction,
                cancellationToken: ct);

            await connectionProvider.Connection.ExecuteAsync(cmd);
        }

        public async Task UpdateAsync( Domain.Reservations.Aggregates.GroupBooking groupBooking, CancellationToken ct = default )
        {
            GroupBookingRow model = GroupBookingMapper.ToPersistenceModel(groupBooking);

            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.Reservations.GroupBooking.Sql.Update.sql");

            CommandDefinition cmd = new(
                sql,
                model,
                connectionProvider.Transaction,
                cancellationToken: ct);

            await connectionProvider.Connection.ExecuteAsync(cmd);
        }
    }
}
