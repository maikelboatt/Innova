using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Persistence;

namespace Innova.Application.Behaviours
{
    public sealed class TransactionCommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> inner,
        IUnitOfWork unitOfWork )
        :ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<TResponse> HandleAsync( TCommand command, CancellationToken ct = default )
        {
            await unitOfWork.BeginAsync(ct);

            try
            {
                TResponse result = await inner.HandleAsync(command, ct);
                await unitOfWork.CommitAsync(ct);
                return result;
            }
            catch
            {
                await unitOfWork.RollbackAsync(ct);
                throw;
            }

        }
    }
}
