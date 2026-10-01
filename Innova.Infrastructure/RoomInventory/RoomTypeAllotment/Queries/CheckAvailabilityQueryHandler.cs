using System.Reflection;
using Dapper;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.RoomInventory.DTO;
using Innova.Application.RoomInventory.Queries;
using Innova.Infrastructure.Persistence;
using Innova.Infrastructure.Persistence.Connections;

namespace Innova.Infrastructure.RoomInventory.RoomTypeAllotment.Queries
{
    public sealed class CheckAvailabilityQueryHandler( IDbConnectionProvider connectionProvider )
        :IQueryHandler<CheckAvailabilityQuery, AvailabilityDto>
    {
        private static readonly Assembly Assembly = typeof(CheckAvailabilityQueryHandler).Assembly;

        public async Task<AvailabilityDto> HandleAsync( CheckAvailabilityQuery query, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.RoomInventory.RoomTypeAllotment.Sql.CheckAvailabilityQuery.sql");

            return await connectionProvider.Connection.QuerySingleAsync<AvailabilityDto>(
                       new CommandDefinition(
                           sql,
                           new
                           {
                               query.RoomTypeId,
                               query.CheckIn,
                               query.CheckOut
                           },
                           connectionProvider.Transaction,
                           cancellationToken: ct));
        }
    }
}
