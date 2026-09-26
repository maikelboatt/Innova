using FluentValidation;

namespace Innova.Application.Billing.Folio.Commands.RecordPayment
{
    public sealed class RecordPaymentCommandValidator:AbstractValidator<RecordPaymentCommand>
    {
        public RecordPaymentCommandValidator()
        {
            RuleFor(x => x.FolioId)
                .NotEmpty()
                .WithMessage("Folio is required.");

            RuleFor(x => x.Amount)
                .GreaterThan(0m)
                .WithMessage("Payment amount must be greater than zero.");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .WithMessage("Currency is required.")
                .Length(3)
                .WithMessage("Currency must be a 3-letter ISO currency code.")
                .Must(BeValidCurrencyCode)
                .WithMessage("Currency must be a valid 3-letter ISO currency code.");

            RuleFor(x => x.Method)
                .IsInEnum()
                .WithMessage("Invalid payment method.");

            RuleFor(x => x.Reference)
                .MaximumLength(100)
                .WithMessage("Payment reference cannot exceed 100 characters.")
                .When(x => x.Reference is not null);
        }

        private static bool BeValidCurrencyCode( string currency ) => currency.Trim()
                                                                              .Length == 3 &&
                                                                      currency.All(char.IsLetter);
    }
}
