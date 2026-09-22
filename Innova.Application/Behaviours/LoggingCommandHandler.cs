using Innova.Application.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Innova.Application.Behaviours
{
    public sealed class LoggingCommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> inner,
        ILogger<LoggingCommandHandler<TCommand, TResponse>> logger )
        :ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<TResponse> HandleAsync(
            TCommand command,
            CancellationToken ct = default )
        {
            logger.LogInformation(
                "Executing command {CommandName}",
                typeof(TCommand).Name);

            try
            {
                TResponse result = await inner.HandleAsync(command, ct);

                logger.LogInformation(
                    "Completed command {CommandName}",
                    typeof(TCommand).Name);

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Command {CommandName} failed",
                    typeof(TCommand).Name);

                throw;
            }
        }
    }
}
