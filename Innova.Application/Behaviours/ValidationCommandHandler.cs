using FluentValidation;
using FluentValidation.Results;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Exceptions;

namespace Innova.Application.Behaviours
{
    /// <summary>
    ///     Wraps every command handler with validation.
    ///     If any validator fails, execution stops — the handler never runs.
    ///     This is the Decorator pattern: same interface, extra behaviour added around it.
    /// </summary>
    public sealed class ValidatingCommandHandler<TCommand, TResult>(
        ICommandHandler<TCommand, TResult> inner,
        IEnumerable<IValidator<TCommand>> validators )
        :ICommandHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        public async Task<TResult> HandleAsync( TCommand command, CancellationToken ct = default )
        {
            if (!validators.Any())
                return await inner.HandleAsync(command, ct);

            // Run all validators and collect failures
            ValidationContext<TCommand> context = new(command);

            IEnumerable<Task<ValidationResult>> validationTasks = validators.Select(v => v.ValidateAsync(context, CancellationToken.None));

            ValidationResult[] results = await Task.WhenAll(validationTasks);

            List<ValidationFailure> failures = results
                                               .SelectMany(r => r.Errors)
                                               .Where(f => f != null)
                                               .ToList();

            if (failures.Count != 0)
                throw new ApplicationValidationException(failures);

            return await inner.HandleAsync(command, ct);
        }
    }
}
