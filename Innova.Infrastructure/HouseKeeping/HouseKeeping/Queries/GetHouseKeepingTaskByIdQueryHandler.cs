using System.Reflection;
using Dapper;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.HouseKeeping.HouseKeeping.DTO;
using Innova.Application.HouseKeeping.HouseKeeping.Queries;
using Innova.Infrastructure.HouseKeeping.HouseKeeping.Mapping;
using Innova.Infrastructure.Persistence;
using Innova.Infrastructure.Persistence.Connections;

namespace Innova.Infrastructure.HouseKeeping.HouseKeeping.Queries
{
    public sealed class GetHouseKeepingTaskByIdQueryHandler( IDbConnectionProvider connectionProvider )
        :IQueryHandler<GetHouseKeepingTaskByIdQuery, HouseKeepingTaskDto?>
    {
        private static readonly Assembly Assembly = typeof(GetHouseKeepingTaskByIdQueryHandler).Assembly;

        public async Task<HouseKeepingTaskDto?> HandleAsync( GetHouseKeepingTaskByIdQuery query, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.HouseKeeping.HouseKeeping.Sql.GetById.sql");

            HouseKeepingTaskRow? row =
                await connectionProvider.Connection.QuerySingleOrDefaultAsync<HouseKeepingTaskRow>(
                    new CommandDefinition(
                        sql,
                        new
                        {
                            Id = query.TaskId
                        },
                        connectionProvider.Transaction,
                        cancellationToken: ct));

            return row is null
                       ? null
                       : HouseKeepingTaskQueryHandlerMapper.ToDto(row);
        }
    }
}
