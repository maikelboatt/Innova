using FluentValidation;

namespace Innova.Application.GuestManagement.Guest.Commands.RegisterGuest
{
    public sealed class RegisterGuestCommandValidator:AbstractValidator<RegisterGuestCommand>
    {
        public RegisterGuestCommandValidator()
        {
            RuleFor(c => c.FirstName)
                .NotEmpty();
            RuleFor(c => c.LastName)
                .NotEmpty();

            RuleFor(c => c.DateOfBirth)
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Date of birth cannot be in the future.");

            RuleFor(c => c.PhoneNumber)
                .NotEmpty();

            RuleFor(c => c.Email)
                .EmailAddress()
                .When(c => !string.IsNullOrWhiteSpace(c.Email));

            RuleFor(c => c.IdentityDocumentNumber)
                .NotEmpty();
        }
    }
}
