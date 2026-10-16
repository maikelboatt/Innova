using FluentValidation;

namespace Innova.Application.Billing.Folio.Commands.IssueRefund
{
    public sealed class IssueRefundCommandValidator:AbstractValidator<IssueRefundCommand>
    {
        public IssueRefundCommandValidator()
        {
            RuleFor(c => c.FolioId)
                .NotEmpty();
            RuleFor(c => c.Amount)
                .GreaterThan(0);
            RuleFor(c => c.Currency)
                .NotEmpty()
                .Length(3);
            RuleFor(c => c.Method)
                .IsInEnum();
            RuleFor(c => c.Reason)
                .NotEmpty()
                .MaximumLength(500);
            RuleFor(c => c.Reference)
                .MaximumLength(200);
        }
    }
}
