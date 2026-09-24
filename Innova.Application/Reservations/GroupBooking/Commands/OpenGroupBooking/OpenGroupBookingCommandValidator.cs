using FluentValidation;

namespace Innova.Application.Reservations.GroupBooking.Commands.OpenGroupBooking
{
    public sealed class OpenGroupBookingCommandValidator:AbstractValidator<OpenGroupBookingCommand>
    {
        public OpenGroupBookingCommandValidator()
        {
            RuleFor(gb => gb.OrganizerGuestId)
                .NotEmpty()
                .WithMessage("Organizer Guest Id is required");

            RuleFor(gb => gb.GroupName)
                .NotEmpty()
                .WithMessage("Group name is required");
        }
    }
}
