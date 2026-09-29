using System.Data;

namespace Innova.Infrastructure.Persistence.Connections
{
    public interface ISqlConnectionFactory
    {
        IDbConnection Create();
    }
}
