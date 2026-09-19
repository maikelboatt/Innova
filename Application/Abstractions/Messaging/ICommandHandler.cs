namespace Application.Abstractions.Messaging
{
    public interface ICommandHandler<TCommand, TResult> where TCommand : ICommand<TResult>
    {
        Task<TResult> HandleAsync( TCommand command, CancellationToken ct = default );
    }

    public interface ICommandHandler<TCommand>:ICommandHandler<TCommand, Unit> where TCommand : ICommand<Unit>
    {
    }
}
