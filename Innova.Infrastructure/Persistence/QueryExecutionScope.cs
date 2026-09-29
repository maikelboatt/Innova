using Innova.Application.Abstractions.Persistence;
using Innova.Infrastructure.Persistence.Connections;
using Innova.Infrastructure.Persistence.Transactions;

namespace Innova.Infrastructure.Persistence
{
    public sealed class QueryExecutionScope( ISqlConnectionFactory connectionFactory ):IQueryExecutionScope
    {
        public async Task<TResult> ExecuteAsync<TResult>(
            Func<Task<TResult>> operation,
            CancellationToken ct = default )
        {
            UnitOfWork unitOfWork = new(connectionFactory);
            UnitOfWork? previous = AmbientUnitOfWork.Current;

            AmbientUnitOfWork.Current = unitOfWork;

            try
            {
                await unitOfWork.OpenAsync(ct);
                return await operation();
            }
            finally
            {
                AmbientUnitOfWork.Current = previous;
                unitOfWork.Dispose();
            }
        }
    }
}
