using System.Reflection;
using Dapper;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.RoomInventory.DTO;
using Innova.Application.RoomInventory.Queries;
using Innova.Infrastructure.Persistence;
using Innova.Infrastructure.Persistence.Connections;
using Innova.Infrastructure.RoomInventory.RoomTypeAllotment.Mapping;

namespace Innova.Infrastructure.RoomInventory.RoomTypeAllotment.Queries
{
    public sealed class GetRoomTypeAllotmentCalendarQueryHandler( IDbConnectionProvider connectionProvider )
        :IQueryHandler<GetRoomTypeAllotmentCalendarQuery, IReadOnlyCollection<AllotmentNightDto>>
    {
        private static readonly Assembly Assembly = typeof(GetRoomTypeAllotmentCalendarQueryHandler).Assembly;

        public async Task<IReadOnlyCollection<AllotmentNightDto>> HandleAsync( GetRoomTypeAllotmentCalendarQuery query, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.RoomInventory.RoomTypeAllotment.Sql.GetRoomTypeAllotmentCalendarQuery.sql");

            IEnumerable<AllotmentNightRow> rows =
                await connectionProvider.Connection.QueryAsync<AllotmentNightRow>(
                    new CommandDefinition(
                        sql,
                        new
                        {
                            query.RoomTypeId,
                            query.From,
                            query.To
                        },
                        connectionProvider.Transaction,
                        cancellationToken: ct));

            return RoomTypeAllotmentQueryHandlerMapper
                .MapToAllotmentNightDto(rows);
        }
    }
}
