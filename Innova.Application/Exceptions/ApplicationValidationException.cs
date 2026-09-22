using FluentValidation.Results;

namespace Innova.Application.Exceptions
{
    public sealed class ApplicationValidationException( IEnumerable<ValidationFailure> failures ):Exception("One or more validation failures occurred")
    {
        public IReadOnlyCollection<ValidationFailure> Errors { get; } = failures
                                                                        .ToList()
                                                                        .AsReadOnly();
    }
}
