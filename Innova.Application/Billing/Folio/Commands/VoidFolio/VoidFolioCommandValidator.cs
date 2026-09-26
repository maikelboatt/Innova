using FluentValidation;

namespace Innova.Application.Billing.Folio.Commands.VoidFolio
{
    public sealed class VoidFolioCommandValidator:AbstractValidator<VoidFolioCommand>
    {
        public VoidFolioCommandValidator()
        {
            RuleFor(f => f.FolioId)
                .NotEmpty()
                .WithMessage("Folio Id is required");
        }
    }
}
