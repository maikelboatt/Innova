using System.Reflection;
using Dapper;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Identity.DTO;
using Innova.Application.Identity.Queries.GetAllUsers;
using Innova.Infrastructure.Persistence;
using Innova.Infrastructure.Persistence.Connections;

namespace Innova.Infrastructure.Identity.User.Queries
{
    public sealed class GetAllUsersHandler( IDbConnectionProvider connectionProvider )
        :IQueryHandler<GetAllUsersQuery, IEnumerable<UserSummaryDto>>
    {
        private static readonly Assembly Assembly = typeof(GetAllUsersHandler).Assembly;

        public async Task<IEnumerable<UserSummaryDto>> HandleAsync( GetAllUsersQuery query, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.Identity.User.Sql.GetAll.sql");

            return await connectionProvider.Connection.QueryAsync<UserSummaryDto>(
                       new CommandDefinition(sql, connectionProvider.Transaction, cancellationToken: ct));
        }
    }
}
