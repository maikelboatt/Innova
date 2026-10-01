using System.Reflection;
using Dapper;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.GuestManagement.Guest.DTO;
using Innova.Application.GuestManagement.Guest.Queries;
using Innova.Infrastructure.GuestManagement.Guest.Mapping;
using Innova.Infrastructure.Persistence;
using Innova.Infrastructure.Persistence.Connections;

namespace Innova.Infrastructure.GuestManagement.Guest.Queries
{
    public sealed class GetAllGuestQueryHandler( IDbConnectionProvider connectionProvider ):IQueryHandler<GetAllGuestQuery, IReadOnlyCollection<GuestDto>>
    {
        private static readonly Assembly Assembly = typeof(GetAllGuestQueryHandler).Assembly;

        public async Task<IReadOnlyCollection<GuestDto>> HandleAsync( GetAllGuestQuery query, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.GuestManagement.Guest.Sql.GetAll.sql");

            IEnumerable<GuestRow> guestTask = await connectionProvider.Connection.QueryAsync<GuestRow>(
                                                  new CommandDefinition(sql, transaction: connectionProvider.Transaction, cancellationToken: ct));


            return GuestQueryHandlerMapper.MapToGuestDto(guestTask);
        }
    }
}
