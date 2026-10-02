using System.Reflection;
using Dapper;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Reservations.DTO;
using Innova.Application.Reservations.Queries;
using Innova.Infrastructure.Persistence;
using Innova.Infrastructure.Persistence.Connections;
using Innova.Infrastructure.Reservations.GroupBooking.Mapping;

namespace Innova.Infrastructure.Reservations.GroupBooking.Queries
{
    public sealed class GetGroupBookingByIdQueryHandler( IDbConnectionProvider connectionProvider )
        :IQueryHandler<GetGroupBookingByIdQuery, GroupBookingDto?>
    {
        private static readonly Assembly Assembly = typeof(GetGroupBookingByIdQueryHandler).Assembly;

        public async Task<GroupBookingDto?> HandleAsync( GetGroupBookingByIdQuery query, CancellationToken cancellationToken = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.Reservations.GroupBooking.Sql.GetById.sql");

            CommandDefinition command = new(
                sql,
                new
                {
                    query.GroupBookingId
                },
                connectionProvider.Transaction,
                cancellationToken: cancellationToken);

            using SqlMapper.GridReader multi =
                await connectionProvider.Connection.QueryMultipleAsync(command);

            GroupBookingRow? groupBookingRow =
                await multi.ReadSingleOrDefaultAsync<GroupBookingRow>();

            if (groupBookingRow is null)
                return null;

            IEnumerable<GroupBookingReservationRow> reservationRows =
                await multi.ReadAsync<GroupBookingReservationRow>();

            GroupBookingRow completeRow = new()
                                          {
                                              Id = groupBookingRow.Id,
                                              OrganizerGuestId = groupBookingRow.OrganizerGuestId,
                                              GroupName = groupBookingRow.GroupName,
                                              ReservationIds = reservationRows
                                                               .Select(row => row.ReservationId)
                                                               .ToList()
                                                               .AsReadOnly()
                                          };

            return GroupBookingQueryHandlerMapper.ToDto(completeRow);
        }
    }
}
