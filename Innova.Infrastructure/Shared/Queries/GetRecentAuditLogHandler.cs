using System.Reflection;
using Dapper;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Shared.DTO;
using Innova.Application.Shared.Queries.GetRecentAuditLog;
using Innova.Infrastructure.Persistence;
using Innova.Infrastructure.Persistence.Connections;

namespace Innova.Infrastructure.Shared.Queries
{
    public sealed class GetRecentAuditLogHandler( IDbConnectionProvider connectionProvider )
        :IQueryHandler<GetRecentAuditLogQuery, IEnumerable<AuditLogDto>>
    {
        private static readonly Assembly Assembly = typeof(GetRecentAuditLogHandler).Assembly;

        public Task<IEnumerable<AuditLogDto>> HandleAsync( GetRecentAuditLogQuery query, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.Shared.Sql.GetRecentAuditLog.sql");

            Task<IEnumerable<AuditLogDto>> rows = connectionProvider.Connection.QueryAsync<AuditLogDto>(
                new CommandDefinition(
                    sql,
                    new
                    {
                        query.Take
                    },
                    connectionProvider.Transaction,
                    cancellationToken: ct));

            return rows;
        }
    }
}
