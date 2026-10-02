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
    public sealed class GetAllGroupBookingsQueryHandler( IDbConnectionProvider connectionProvider )
        :IQueryHandler<GetAllGroupBookingsQuery, IReadOnlyCollection<GroupBookingDto>>
    {
        private static readonly Assembly Assembly = typeof(GetAllGroupBookingsQueryHandler).Assembly;

        public async Task<IReadOnlyCollection<GroupBookingDto>> HandleAsync( GetAllGroupBookingsQuery query, CancellationToken cancellationToken = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.Reservations.GroupBooking.Sql.GetAll.sql");

            CommandDefinition command = new(
                sql,
                transaction: connectionProvider.Transaction,
                cancellationToken: cancellationToken);

            using SqlMapper.GridReader multi = await connectionProvider.Connection.QueryMultipleAsync(command);

            IEnumerable<GroupBookingRow> groupRows = await multi.ReadAsync<GroupBookingRow>();

            IEnumerable<GroupBookingReservationRow> reservationRows = await multi.ReadAsync<GroupBookingReservationRow>();

            Dictionary<Guid, IReadOnlyCollection<Guid>> reservationsByGroup =
                reservationRows
                    .GroupBy(row => row.GroupBookingId)
                    .ToDictionary(
                        group => group.Key,
                        group => (IReadOnlyCollection<Guid>)group
                                                            .Select(row => row.ReservationId)
                                                            .ToList()
                                                            .AsReadOnly());

            List<GroupBookingRow> completeRows =
                groupRows
                    .Select(group => new GroupBookingRow
                                     {
                                         Id = group.Id,
                                         OrganizerGuestId = group.OrganizerGuestId,
                                         GroupName = group.GroupName,
                                         ReservationIds =
                                             reservationsByGroup.TryGetValue(
                                                 group.Id,
                                                 out IReadOnlyCollection<Guid>? reservationIds)
                                                 ? reservationIds
                                                 : []
                                     })
                    .ToList();

            return GroupBookingQueryHandlerMapper.MapToDto(completeRows);
        }
    }
}
