using FluentValidation;

namespace Innova.Application.Billing.Folio.Commands.SettleFolio
{
    public sealed class SettleFolioCommandValidator:AbstractValidator<SettleFolioCommand>
    {
        public SettleFolioCommandValidator()
        {
            RuleFor(f => f.FolioId)
                .NotEmpty()
                .WithMessage("Folio Id is required");
        }
    }
}
