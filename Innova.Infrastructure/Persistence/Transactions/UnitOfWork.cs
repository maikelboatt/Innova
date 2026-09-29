// MediCore.Infrastructure/Persistence/UnitOfWork.cs

using System.Data;
using System.Data.Common;
using Innova.Application.Abstractions.Persistence;
using Innova.Infrastructure.Persistence.Connections;
using Microsoft.Data.SqlClient;

namespace Innova.Infrastructure.Persistence.Transactions
{
    public sealed class UnitOfWork( ISqlConnectionFactory connectionFactory ):IUnitOfWork, IDbConnectionProvider
    {
        public IDbConnection Connection { get; private set; } = default!;
        public IDbTransaction? Transaction { get; private set; }

        public async Task BeginAsync( CancellationToken ct = default )
        {
            await OpenAsync(ct);
            Transaction = Connection.BeginTransaction();
        }

        public async Task CommitAsync( CancellationToken ct = default )
        {
            if (Transaction is SqlTransaction sqlTransaction)
                await sqlTransaction.CommitAsync(ct);

            await DisposeConnectionAsync();
        }

        public async Task RollbackAsync( CancellationToken ct = default )
        {
            if (Transaction is SqlTransaction sqlTransaction)
                await sqlTransaction.RollbackAsync(ct);

            await DisposeConnectionAsync();
        }

        public void Dispose()
        {

            Transaction?.Dispose();
            Connection?.Dispose();
        }

        public async Task OpenAsync( CancellationToken ct = default )
        {
            if (Connection is not null)
                return;

            Connection = connectionFactory.Create();

            if (Connection is DbConnection dbConnection)
                await dbConnection.OpenAsync(ct);
            else
                Connection.Open();
        }

        private async Task DisposeConnectionAsync()
        {
            Transaction?.Dispose();
            Transaction = null;

            if (Connection is SqlConnection sqlConnection)
                await sqlConnection.DisposeAsync();
        }
    }
}
