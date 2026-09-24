using FluentValidation;

namespace Innova.Application.FrontDesk.Reservation.Commands.CheckOut
{
    public sealed class CheckOutCommandValidator:AbstractValidator<CheckOutCommand>
    {
        public CheckOutCommandValidator()
        {
            RuleFor(s => s.StayId)
                .NotEmpty()
                .WithMessage("Stay Id is required");
        }
    }
}
