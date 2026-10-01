using System.Reflection;
using Dapper;
using Innova.Domain.GuestManagement.Repositories;
using Innova.Domain.GuestManagement.ValueObjects;
using Innova.Infrastructure.GuestManagement.Guest.Mapping;
using Innova.Infrastructure.Persistence;
using Innova.Infrastructure.Persistence.Connections;

namespace Innova.Infrastructure.GuestManagement.Guest.Repository
{
    public sealed class GuestRepository( IDbConnectionProvider connectionProvider ):IGuestRepository
    {
        private static readonly Assembly Assembly = typeof(GuestRepository).Assembly;

        public async Task<Domain.GuestManagement.Aggregates.Guest?> GetByIdAsync( GuestId guestId, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.GuestManagement.Guest.Sql.GetById.sql");

            GuestRow? guestRow = await connectionProvider.Connection.QuerySingleOrDefaultAsync<GuestRow>(
                                     new CommandDefinition(
                                         sql,
                                         new
                                         {
                                             Id = guestId.Value
                                         },
                                         connectionProvider.Transaction,
                                         cancellationToken: ct));


            return guestRow is null
                       ? null
                       : GuestMapper.ToDomain(guestRow);
        }

        public async Task SaveAsync( Domain.GuestManagement.Aggregates.Guest guest, CancellationToken ct = default )
        {
            GuestRow model = GuestMapper.ToPersistenceModel(guest);

            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.GuestManagement.Guest.Sql.Insert.sql");

            CommandDefinition cmd = new(
                sql,
                model,
                connectionProvider.Transaction,
                cancellationToken: ct);

            await connectionProvider.Connection.ExecuteAsync(cmd);
        }

        public async Task UpdateAsync( Domain.GuestManagement.Aggregates.Guest guest, CancellationToken ct = default )
        {
            GuestRow model = GuestMapper.ToPersistenceModel(guest);

            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.GuestManagement.Guest.Sql.Update.sql");

            CommandDefinition cmd = new(
                sql,
                model,
                connectionProvider.Transaction,
                cancellationToken: ct);

            await connectionProvider.Connection.ExecuteAsync(cmd);
        }

        public async Task<bool> ExistsWithIdentityDocumentAsync( IdentityDocument identityDocument, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.GuestManagement.Guest.Sql.ExistByIdentityDocument.sql");

            return await connectionProvider.Connection.ExecuteScalarAsync<bool>(
                       new CommandDefinition(
                           sql,
                           new
                           {
                               IdentityDocumentType = identityDocument.Type.ToString(),
                               IdentityDocumentNumber = identityDocument.Number
                           },
                           connectionProvider.Transaction,
                           cancellationToken: ct));
        }
    }
}
