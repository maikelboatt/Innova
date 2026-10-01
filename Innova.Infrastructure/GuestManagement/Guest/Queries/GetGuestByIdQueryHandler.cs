using System.Reflection;
using Dapper;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.GuestManagement.Guest.DTO;
using Innova.Application.GuestManagement.Guest.Exceptions;
using Innova.Application.GuestManagement.Guest.Queries;
using Innova.Domain.GuestManagement.ValueObjects;
using Innova.Infrastructure.GuestManagement.Guest.Mapping;
using Innova.Infrastructure.Persistence;
using Innova.Infrastructure.Persistence.Connections;

namespace Innova.Infrastructure.GuestManagement.Guest.Queries
{
    public sealed class GetGuestByIdQueryHandler( IDbConnectionProvider connectionProvider ):IQueryHandler<GetGuestByIdQuery, GuestDto>
    {
        private static readonly Assembly Assembly = typeof(GetGuestByIdQueryHandler).Assembly;

        public async Task<GuestDto> HandleAsync( GetGuestByIdQuery query, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.GuestManagement.Guest.Sql.GetById.sql");

            GuestRow? guestRow = await connectionProvider.Connection.QuerySingleOrDefaultAsync<GuestRow>(
                                     new CommandDefinition(
                                         sql,
                                         new
                                         {
                                             Id = query.GuestId
                                         },
                                         connectionProvider.Transaction,
                                         cancellationToken: ct));


            return guestRow is null
                       ? throw new GuestNotFoundException(GuestId.From(query.GuestId))
                       : GuestQueryHandlerMapper.ToDto(guestRow);
        }
    }
}
