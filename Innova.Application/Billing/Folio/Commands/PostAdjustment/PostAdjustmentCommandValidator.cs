using FluentValidation;

namespace Innova.Application.Billing.Folio.Commands.PostAdjustment
{
    public sealed class PostAdjustmentCommandValidator:AbstractValidator<PostAdjustmentCommand>
    {
        public PostAdjustmentCommandValidator()
        {
            RuleFor(f => f.FolioId)
                .NotEmpty()
                .WithMessage("Folio Id is required");

            RuleFor(x => x.Amount)
                .GreaterThan(0m)
                .WithMessage("Adjustment amount must be greater than zero.");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .WithMessage("Currency is required.")
                .Length(3)
                .WithMessage("Currency must be a 3-letter ISO currency code.")
                .Must(BeValidCurrencyCode)
                .WithMessage("Currency must be a valid 3-letter ISO currency code.");

            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("Invalid adjustment type.");

            RuleFor(x => x.Reason)
                .NotEmpty()
                .WithMessage("Adjustment reason is required.")
                .MaximumLength(500)
                .WithMessage("Adjustment reason cannot exceed 500 characters.");
        }

        private static bool BeValidCurrencyCode( string currency ) => currency.Trim()
                                                                              .Length == 3 &&
                                                                      currency.All(char.IsLetter);
    }
}
