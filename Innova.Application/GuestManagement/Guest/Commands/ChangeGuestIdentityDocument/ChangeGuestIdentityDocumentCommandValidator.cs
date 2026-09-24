using FluentValidation;

namespace Innova.Application.GuestManagement.Guest.Commands.ChangeGuestIdentityDocument
{
    public class ChangeGuestIdentityDocumentCommandValidator:AbstractValidator<ChangeGuestIdentityDocumentCommand>
    {
        public ChangeGuestIdentityDocumentCommandValidator()
        {
            RuleFor(c => c.GuestId)
                .NotEmpty()
                .WithMessage("Guest is required.");
            RuleFor(c => c.IdentityDocumentNumber)
                .NotEmpty()
                .WithMessage("Identity document is required.");
        }
    }
}
