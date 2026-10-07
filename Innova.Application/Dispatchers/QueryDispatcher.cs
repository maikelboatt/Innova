using System.Reflection;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Persistence;
using MvvmCross.IoC;

namespace Innova.Application.Dispatchers
{
    public class QueryDispatcher( IMvxIoCProvider iocProvider, IQueryExecutionScope queryExecutionScope )
    {
        public async Task<TResult> DispatchAsync<TResult>( IQuery<TResult> query, CancellationToken ct = default )
        {
            return await queryExecutionScope.ExecuteAsync(
                       async () =>
                       {
                           Type handlerType = typeof(IQueryHandler<,>)
                               .MakeGenericType(query.GetType(), typeof(TResult));

                           dynamic handler = iocProvider.Resolve(handlerType);

                           MethodInfo method = handlerType.GetMethod(
                               nameof(IQueryHandler<IQuery<TResult>, TResult>.HandleAsync))!;

                           Task<TResult> result = (Task<TResult>)method.Invoke(
                               handler,
                               new object[]
                               {
                                   query, ct
                               })!;

                           return await result;
                       },
                       ct);
        }
    }
}
