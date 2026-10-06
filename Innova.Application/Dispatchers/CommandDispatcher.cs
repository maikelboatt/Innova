using Innova.Application.Abstractions.Messaging;
using MvvmCross.IoC;

namespace Innova.Application.Dispatchers
{
    /// <summary>
    ///     Resolves the correct ICommandHandler for a given ICommand and invokes it.
    ///     The ViewModel calls this — it never knows which handler runs.
    /// </summary>
    public sealed class CommandDispatcher( IMvxIoCProvider iocProvider )
    {
        public async Task<TResult> SendAsync<TCommand, TResult>( TCommand command, CancellationToken ct = default )
            where TCommand : ICommand<TResult>
        {
            ICommandHandler<TCommand, TResult> handler =
                iocProvider.Resolve<ICommandHandler<TCommand, TResult>>()
                ?? throw new InvalidOperationException(
                    $"No handler registered for: {typeof(TCommand).Name}");

            return await handler.HandleAsync(command, ct);
        }

        public async Task SendAsync<TCommand>( TCommand command, CancellationToken ct = default )
            where TCommand : ICommand // ← concrete ICommand not ICommand<Unit>
        {
            await SendAsync<TCommand, Unit>(command, ct);
        }
    }
}
