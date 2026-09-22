using FluentValidation.Results;

namespace Innova.Application.Abstractions.Validation
{
    public interface IValidator<in T>
    {
        Task<ValidationResult> ValidateAsync( T instance, CancellationToken ct = default );
    }
}
