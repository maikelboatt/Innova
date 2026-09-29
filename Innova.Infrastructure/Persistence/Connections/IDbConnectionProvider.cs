using System.Data;

namespace Innova.Infrastructure.Persistence.Connections
{
    public interface IDbConnectionProvider
    {
        IDbConnection Connection { get; }
        IDbTransaction? Transaction { get; }
    }
}
