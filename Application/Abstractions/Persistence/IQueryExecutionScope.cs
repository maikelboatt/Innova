namespace Application.Abstractions.Persistence
{
    public interface IQueryExecutionScope
    {
        Task<TResult> ExecuteAsync<TResult>( Func<Task<TResult>> operation, CancellationToken ct = default );
    }
}
