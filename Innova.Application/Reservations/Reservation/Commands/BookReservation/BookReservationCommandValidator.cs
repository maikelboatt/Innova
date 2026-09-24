using FluentValidation;

namespace Innova.Application.Reservations.Reservation.Commands.BookReservation
{
    public sealed class BookReservationCommandValidator:AbstractValidator<BookReservationCommand>
    {
        public BookReservationCommandValidator()
        {
            RuleFor(x => x.GuestId)
                .NotEmpty()
                .WithMessage("Guest is required.");

            RuleFor(x => x.RoomTypeRequested)
                .NotEmpty()
                .WithMessage("Room type is required.");

            RuleFor(x => x.CheckIn)
                .Must(date => date != default)
                .WithMessage("Check-in date is required.");

            RuleFor(x => x.CheckOut)
                .Must(date => date != default)
                .WithMessage("Check-out date is required.");

            RuleFor(x => x)
                .Must(x => x.CheckOut > x.CheckIn)
                .WithMessage("Check-out date must be after check-in date.");

            RuleFor(x => x.NightlyRateAmount)
                .GreaterThanOrEqualTo(0m)
                .WithMessage("Nightly rate cannot be negative.");

            RuleFor(x => x.NightlyRateCurrency)
                .NotEmpty()
                .WithMessage("Nightly rate currency is required.")
                .Length(3)
                .WithMessage("Nightly rate currency must be a 3-letter ISO currency code.");

            RuleFor(x => x.FreeCancellationWindowHours)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Free cancellation window cannot be negative.");

            RuleFor(x => x.CancellationFeeAmount)
                .GreaterThanOrEqualTo(0m)
                .WithMessage("Cancellation fee cannot be negative.");

            RuleFor(x => x.CancellationFeeCurrency)
                .NotEmpty()
                .WithMessage("Cancellation fee currency is required.")
                .Length(3)
                .WithMessage("Cancellation fee currency must be a 3-letter ISO currency code.");

            RuleFor(x => x.CancellationFeeCurrency)
                .Must(( command, currency ) =>
                          string.Equals(
                              currency.Trim(),
                              command.NightlyRateCurrency.Trim(),
                              StringComparison.OrdinalIgnoreCase))
                .WithMessage("Cancellation fee currency must match the nightly rate currency.");
        }
    }
}
