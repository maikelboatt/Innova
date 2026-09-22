using FluentValidation;

namespace Innova.Application.GuestManagement.Guest.Commands.UpdateGuestContactDetails
{
    public class UpdateGuestContactDetailsCommandValidator:AbstractValidator<UpdateGuestContactDetailsCommand>
    {
        public UpdateGuestContactDetailsCommandValidator()
        {
            RuleFor(c => c.GuestId)
                .NotEmpty();
            RuleFor(c => c.PhoneNumber)
                .NotEmpty();

            RuleFor(c => c.Email)
                .EmailAddress()
                .When(c => !string.IsNullOrWhiteSpace(c.Email));
        }
    }
}
