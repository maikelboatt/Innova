using FluentValidation;

namespace Innova.Application.Billing.Folio.Commands.PostCharge
{
    public sealed class PostChargeCommandValidator:AbstractValidator<PostChargeCommand>
    {
        public PostChargeCommandValidator()
        {
            RuleFor(x => x.FolioId)
                .NotEmpty()
                .WithMessage("Folio is required.");

            RuleFor(x => x.Amount)
                .GreaterThan(0m)
                .WithMessage("Charge amount must be greater than zero.");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .WithMessage("Currency is required.")
                .Length(3)
                .WithMessage("Currency must be a 3-letter ISO currency code.")
                .Must(BeValidCurrencyCode)
                .WithMessage("Currency must be a valid 3-letter ISO currency code.");

            RuleFor(x => x.Category)
                .NotEmpty()
                .WithMessage("Charge category is required.")
                .MaximumLength(100)
                .WithMessage("Charge category cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Charge description is required.")
                .MaximumLength(500)
                .WithMessage("Charge description cannot exceed 500 characters.");
        }

        private static bool BeValidCurrencyCode( string currency ) => currency.Trim()
                                                                              .Length == 3 &&
                                                                      currency.All(char.IsLetter);
    }
}
