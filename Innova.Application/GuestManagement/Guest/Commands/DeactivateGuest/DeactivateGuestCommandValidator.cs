using FluentValidation;

namespace Innova.Application.GuestManagement.Guest.Commands.DeactivateGuest
{
    public class DeactivateGuestCommandValidator:AbstractValidator<DeactivateGuestCommand>
    {
        public DeactivateGuestCommandValidator()
        {
            RuleFor(c => c.GuestId)
                .NotEmpty();
        }
    }
}
