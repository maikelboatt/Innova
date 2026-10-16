using System.Data;
using System.Reflection;
using Dapper;
using Innova.Domain.Identity.Repositories;
using Innova.Domain.Shared.ValueObjects;
using Innova.Infrastructure.Identity.User.Mapping;
using Innova.Infrastructure.Persistence;
using Innova.Infrastructure.Persistence.Connections;

namespace Innova.Infrastructure.Identity.User.Repository
{
    public sealed class UserRepository( IDbConnectionProvider connectionProvider ):IUserRepository
    {
        private static readonly Assembly Assembly = typeof(UserRepository).Assembly;

        public async Task<Domain.Identity.Aggregates.User?> GetByIdAsync( UserId userId, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.Identity.User.Sql.GetById.sql");
            IDbConnection connection = connectionProvider.Connection
                                       ?? throw new InvalidOperationException(
                                           "The database connection is null in UserRepository.");

            UserRow? row = await connection.QuerySingleOrDefaultAsync<UserRow>(
                               new CommandDefinition(
                                   sql,
                                   new
                                   {
                                       Id = userId.Value
                                   },
                                   connectionProvider.Transaction,
                                   cancellationToken: ct));

            return row is null
                       ? null
                       : UserMapper.ToDomain(row);
        }

        public async Task<Domain.Identity.Aggregates.User?> GetByUsernameAsync( string username, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.Identity.User.Sql.GetByUsername.sql");

            UserRow? row = await connectionProvider.Connection.QuerySingleOrDefaultAsync<UserRow>(
                               new CommandDefinition(
                                   sql,
                                   new
                                   {
                                       Username = username
                                   },
                                   connectionProvider.Transaction,
                                   cancellationToken: ct));

            return row is null
                       ? null
                       : UserMapper.ToDomain(row);
        }

        public async Task SaveAsync( Domain.Identity.Aggregates.User user, CancellationToken ct = default )
        {
            UserRow row = UserMapper.ToPersistenceModel(user);
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.Identity.User.Sql.Insert.sql");

            await connectionProvider.Connection.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    row,
                    connectionProvider.Transaction,
                    cancellationToken: ct));
        }

        public async Task UpdateAsync( Domain.Identity.Aggregates.User user, CancellationToken ct = default )
        {
            UserRow row = UserMapper.ToPersistenceModel(user);
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.Identity.User.Sql.Update.sql");

            await connectionProvider.Connection.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    row,
                    connectionProvider.Transaction,
                    cancellationToken: ct));
        }
    }
}
