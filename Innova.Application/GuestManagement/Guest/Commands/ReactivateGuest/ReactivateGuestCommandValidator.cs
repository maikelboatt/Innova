using FluentValidation;

namespace Innova.Application.GuestManagement.Guest.Commands.ReactivateGuest
{
    public class ReactivateGuestCommandValidator:AbstractValidator<ReactivateGuestCommand>
    {
        public ReactivateGuestCommandValidator()
        {
            RuleFor(c => c.GuestId)
                .NotEmpty()
                .WithMessage("Guest is required.");
        }
    }
}
