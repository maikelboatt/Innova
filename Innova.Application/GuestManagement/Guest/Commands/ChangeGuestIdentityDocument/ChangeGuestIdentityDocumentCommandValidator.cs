using FluentValidation;

namespace Innova.Application.GuestManagement.Guest.Commands.ChangeGuestIdentityDocument
{
    public class ChangeGuestIdentityDocumentCommandValidator:AbstractValidator<ChangeGuestIdentityDocumentCommand>
    {
        public ChangeGuestIdentityDocumentCommandValidator()
        {
            RuleFor(c => c.GuestId)
                .NotEmpty();
            RuleFor(c => c.IdentityDocumentNumber)
                .NotEmpty();
        }
    }
}
