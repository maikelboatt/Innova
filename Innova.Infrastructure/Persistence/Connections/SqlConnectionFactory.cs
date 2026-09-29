using System.Data;
using Microsoft.Data.SqlClient;

namespace Innova.Infrastructure.Persistence.Connections
{
    public sealed class SqlConnectionFactory( string connectionString ):ISqlConnectionFactory
    {
        public IDbConnection Create() => new SqlConnection(connectionString);
    }
}
