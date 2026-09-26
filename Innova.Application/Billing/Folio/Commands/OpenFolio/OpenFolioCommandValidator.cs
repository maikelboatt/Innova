using FluentValidation;

namespace Innova.Application.Billing.Folio.Commands.OpenFolio
{
    public sealed class OpenFolioCommandValidator:AbstractValidator<OpenFolioCommand>
    {
        public OpenFolioCommandValidator()
        {
            RuleFor(x => x.OwnerId)
                .NotEmpty()
                .WithMessage("Folio owner is required.");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .WithMessage("Currency is required.")
                .Length(3)
                .WithMessage("Currency must be a 3-letter ISO currency code.")
                .Must(BeValidCurrencyCode)
                .WithMessage("Currency must be a valid 3-letter ISO currency code.");

            RuleFor(x => x.OwnerType)
                .IsInEnum()
                .WithMessage("Invalid folio owner type.");
        }

        private static bool BeValidCurrencyCode( string currency ) => currency.Trim()
                                                                              .Length == 3 &&
                                                                      currency.All(char.IsLetter);
    }
}
