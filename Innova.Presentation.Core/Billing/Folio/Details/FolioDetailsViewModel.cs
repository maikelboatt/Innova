using Innova.Application.Abstractions.Messaging;
using Innova.Application.Billing.Folio.Commands.IssueRefund;
using Innova.Application.Billing.Folio.Commands.PostAdjustment;
using Innova.Application.Billing.Folio.Commands.PostCharge;
using Innova.Application.Billing.Folio.Commands.RecordPayment;
using Innova.Application.Billing.Folio.Commands.SettleFolio;
using Innova.Application.Billing.Folio.Commands.VoidFolio;
using Innova.Application.Billing.Folio.DTO;
using Innova.Application.Billing.Folio.Queries;
using Innova.Application.Dispatchers;
using Innova.Domain.Billing.ValueObjects;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Billing.Folio.Lists;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.Commands;
// ICommand / Unit
// Command namespaces below: adjust if yours live in Innova.Application.Billing.Commands.

namespace Innova.Presentation.Core.Billing.Folio.Details
{
    /// <summary>
    ///     A folio's statement: who it is for, what is owed, every charge/payment/adjustment, and the actions
    ///     (post a charge, record a payment, post a credit, settle, void). The entry forms are inline panels
    ///     instead of nested modals, so several postings can be made in a row without leaving the screen.
    /// </summary>
    public sealed class FolioDetailsViewModel:DetailsViewModel<Guid>
    {
        private const string NoComposer = "None";
        private const string ChargeComposerName = "Charge";
        private const string PaymentComposerName = "Payment";
        private const string AdjustmentComposerName = "Adjustment";
        private const string RefundComposerName = "Refund";

        private static readonly string[] DerivedProperties =
        [
            nameof(HasEntries), nameof(ShowEmptyLedger), nameof(IsOpen), nameof(BalanceCaption),
            nameof(CanSettle), nameof(SettleHint), nameof(CanVoid),
            nameof(IsComposerOpen), nameof(IsChargeComposerOpen), nameof(IsPaymentComposerOpen), nameof(IsAdjustmentComposerOpen),
            nameof(CanSubmitCharge), nameof(CanSubmitPayment), nameof(CanSubmitAdjustment), nameof(CanSubmitRefund),
            nameof(CanRefund), nameof(RefundHint), nameof(RefundableDisplay), nameof(IsRefundComposerOpen)
        ];

        private readonly IMessageBoxService _messageBoxService;
        private readonly INotificationService _notifications;
        private readonly QueryDispatcher _queryDispatcher;

        private string _activeComposer = NoComposer;
        private IReadOnlyList<FolioLedgerEntryViewModel> _entries = [];
        private bool _isSubmitting;
        private decimal _refundable;
        private FolioListItemViewModel? _summary;

        public FolioDetailsViewModel( CommandDispatcher commandDispatcher,
                                      IBusyIndicatorService busyIndicator,
                                      INotificationService notifications,
                                      IModalService modalService,
                                      QueryDispatcher queryDispatcher,
                                      IMessageBoxService messageBoxService ):base(
            commandDispatcher,
            busyIndicator,
            notifications,
            modalService)
        {
            _notifications = notifications;
            _queryDispatcher = queryDispatcher;
            _messageBoxService = messageBoxService;

            ChargeComposer.Changed += OnComposerChanged;
            PaymentComposer.Changed += OnComposerChanged;
            AdjustmentComposer.Changed += OnComposerChanged;
            RefundComposer.Changed += OnComposerChanged;

            ShowChargeComposerCommand = new MvxCommand(() => ToggleComposer(ChargeComposerName));
            ShowPaymentComposerCommand = new MvxCommand(() => ToggleComposer(PaymentComposerName));
            ShowAdjustmentComposerCommand = new MvxCommand(() => ToggleComposer(AdjustmentComposerName));
            ShowRefundComposerCommand = new MvxCommand(() => ToggleComposer(RefundComposerName));
            CloseComposerCommand = new MvxCommand(() => ToggleComposer(NoComposer));

            SelectCategoryCommand = new MvxCommand<string>(category => ChargeComposer.Category = category);
            SelectPaymentMethodCommand = new MvxCommand<ComposerChoiceViewModel<PaymentMethod>>(PaymentComposer.SelectMethod);
            SelectAdjustmentTypeCommand = new MvxCommand<ComposerChoiceViewModel<AdjustmentType>>(AdjustmentComposer.SelectType);
            SelectRefundMethodCommand = new MvxCommand<ComposerChoiceViewModel<PaymentMethod>>(RefundComposer.SelectMethod);

            SubmitChargeCommand = new MvxAsyncCommand(SubmitChargeAsync, () => CanSubmitCharge);
            SubmitPaymentCommand = new MvxAsyncCommand(SubmitPaymentAsync, () => CanSubmitPayment);
            SubmitAdjustmentCommand = new MvxAsyncCommand(SubmitAdjustmentAsync, () => CanSubmitAdjustment);
            SubmitRefundCommand = new MvxAsyncCommand(SubmitRefundAsync, () => CanSubmitRefund);
            SettleCommand = new MvxAsyncCommand(SettleAsync, () => CanSettle && !_isSubmitting);
            VoidCommand = new MvxAsyncCommand(VoidAsync, () => CanVoid && !_isSubmitting);
        }

        // ── Statement ────────────────────────────────────────────
        public FolioListItemViewModel? Summary
        {
            get => _summary;
            private set => SetProperty(ref _summary, value);
        }

        public IReadOnlyList<FolioLedgerEntryViewModel> Entries
        {
            get => _entries;
            private set => SetProperty(ref _entries, value);
        }

        public bool HasEntries => Entries.Count > 0;
        public bool ShowEmptyLedger => !HasEntries;

        public bool IsOpen => Summary?.Status == "Open";

        public string BalanceCaption => Summary switch
                                        {
                                            null                  => string.Empty,
                                            { Status: "Void" }    => "Voided",
                                            { Balance: > 0 }      => "Balance due",
                                            { Balance: < 0 }      => "Credit balance",
                                            { Status: "Settled" } => "Settled in full",
                                            _                     => "Nothing owed"
                                        };

        // ── Actions available right now ──────────────────────────
        public bool CanSettle => IsOpen && Summary is { Balance: 0 };

        public string SettleHint => Summary switch
                                    {
                                        null                  => string.Empty,
                                        { Status: "Settled" } => "This folio is already settled.",
                                        { Status: "Void" }    => "A voided folio cannot be settled.",
                                        { Balance: > 0 }      => $"Collect the remaining {Summary.BalanceDisplay} first. A folio can only be settled at zero.",
                                        { Balance: < 0 } => CanRefund
                                                                ? "The folio is in credit. Refund it first (+ Refund), then settle."
                                                                : "The folio is in credit, but no cash was received for it, so there is nothing to refund. Review the credit before settling.",
                                        _ => "Mark the folio as settled."
                                    };

        public bool CanVoid => IsOpen;

        /// <summary>A refund is possible only while the folio is open and holds cash that can be handed back.</summary>
        public bool CanRefund => IsOpen && _refundable > 0;

        public string RefundableDisplay => Summary is { } folio
                                               ? $"{folio.Currency} {_refundable:N2}"
                                               : string.Empty;

        public string RefundHint => CanRefund
                                        ? $"Hand back up to {RefundableDisplay}."
                                        : "Nothing to refund: a refund needs the folio to be in credit, with cash received for it.";

        // ── Inline entry forms ───────────────────────────────────
        public ChargeComposerViewModel ChargeComposer { get; } = new();
        public PaymentComposerViewModel PaymentComposer { get; } = new();
        public AdjustmentComposerViewModel AdjustmentComposer { get; } = new();
        public RefundComposerViewModel RefundComposer { get; } = new();

        public bool IsComposerOpen => _activeComposer != NoComposer;
        public bool IsChargeComposerOpen => _activeComposer == ChargeComposerName;
        public bool IsPaymentComposerOpen => _activeComposer == PaymentComposerName;
        public bool IsAdjustmentComposerOpen => _activeComposer == AdjustmentComposerName;
        public bool IsRefundComposerOpen => _activeComposer == RefundComposerName;

        public bool CanSubmitCharge => IsOpen && ChargeComposer.IsValid && !_isSubmitting;
        public bool CanSubmitPayment => IsOpen && PaymentComposer.IsValid && !_isSubmitting;
        public bool CanSubmitAdjustment => IsOpen && AdjustmentComposer.IsValid && !_isSubmitting;
        public bool CanSubmitRefund => CanRefund && RefundComposer.IsValid && !_isSubmitting;

        // ── Commands ─────────────────────────────────────────────
        public IMvxCommand ShowChargeComposerCommand { get; }
        public IMvxCommand ShowPaymentComposerCommand { get; }
        public IMvxCommand ShowAdjustmentComposerCommand { get; }
        public IMvxCommand ShowRefundComposerCommand { get; }
        public IMvxCommand CloseComposerCommand { get; }
        public IMvxCommand<string> SelectCategoryCommand { get; }
        public IMvxCommand<ComposerChoiceViewModel<PaymentMethod>> SelectPaymentMethodCommand { get; }
        public IMvxCommand<ComposerChoiceViewModel<AdjustmentType>> SelectAdjustmentTypeCommand { get; }
        public IMvxCommand<ComposerChoiceViewModel<PaymentMethod>> SelectRefundMethodCommand { get; }
        public IMvxAsyncCommand SubmitChargeCommand { get; }
        public IMvxAsyncCommand SubmitPaymentCommand { get; }
        public IMvxAsyncCommand SubmitAdjustmentCommand { get; }
        public IMvxAsyncCommand SubmitRefundCommand { get; }
        public IMvxAsyncCommand SettleCommand { get; }
        public IMvxAsyncCommand VoidCommand { get; }

        // ── Loading ──────────────────────────────────────────────
        protected override async Task LoadAsync()
        {
            FolioListItemDto? header = await _queryDispatcher.DispatchAsync(new GetFolioSummaryQuery(Key));
            FolioDto? folio = await _queryDispatcher.DispatchAsync(new GetFolioByIdQuery(Key));

            if (header is null || folio is null)
            {
                _notifications.ShowError("This folio no longer exists.");
                ModalService.Close();
                return;
            }

            Summary = new FolioListItemViewModel(header);

            Entries =
            [
                .. folio
                   .Charges.Select(FolioLedgerEntryViewModel.FromCharge)
                   .Concat(folio.Payments.Select(FolioLedgerEntryViewModel.FromPayment))
                   .Concat(folio.Refunds.Select(FolioLedgerEntryViewModel.FromRefund))
                   .Concat(folio.Adjustments.Select(FolioLedgerEntryViewModel.FromAdjustment))
                   .OrderByDescending(entry => entry.OccurredAt)
            ];

            PaymentComposer.SetContext(header.Currency, header.Balance);

            // What may be handed back: the lesser of the folio's credit and the cash actually received
            // (payments minus refunds), never below zero. This mirrors Folio.Refundable in the domain, which is
            // what finally enforces it: this value only decides whether the button and the amount limit appear.
            decimal credit = Math.Max(0m, -header.Balance);
            decimal netReceived = folio.Payments.Sum(payment => payment.Amount) - folio.Refunds.Sum(refund => refund.Amount);
            _refundable = Math.Max(0m, Math.Min(credit, netReceived));
            RefundComposer.SetContext(header.Currency, _refundable);

            // A folio that is no longer open cannot take entries, so close any panel left open.
            if (!IsOpen) _activeComposer = NoComposer;

            RaiseState();
        }

        // ── Entry forms ──────────────────────────────────────────
        private void ToggleComposer( string composer )
        {
            _activeComposer = _activeComposer == composer
                                  ? NoComposer
                                  : composer;
            RaiseState();
        }

        private async Task SubmitChargeAsync()
        {
            if (!CanSubmitCharge || Summary is not { } folio || ChargeComposer.Amount is not { } amount) return;

            string category = ChargeComposer.Category.Trim();
            string description = string.IsNullOrWhiteSpace(ChargeComposer.Description)
                                     ? category
                                     : ChargeComposer.Description.Trim();

            await SubmitAsync(
                new PostChargeCommand(
                    Key,
                    amount,
                    folio.Currency,
                    category,
                    description),
                $"{folio.Currency} {amount:N2} charged to {category}.",
                ChargeComposer.Reset);
        }

        private async Task SubmitPaymentAsync()
        {
            if (!CanSubmitPayment || Summary is not { } folio
                                  || PaymentComposer.Amount is not { } amount
                                  || PaymentComposer.SelectedMethod is not { } method) return;

            string? reference = string.IsNullOrWhiteSpace(PaymentComposer.Reference)
                                    ? null
                                    : PaymentComposer.Reference.Trim();

            await SubmitAsync(
                new RecordPaymentCommand(
                    Key,
                    amount,
                    folio.Currency,
                    method.Value,
                    reference),
                $"{folio.Currency} {amount:N2} payment recorded ({method.Label.ToLowerInvariant()}).",
                PaymentComposer.Reset);
        }

        private async Task SubmitAdjustmentAsync()
        {
            if (!CanSubmitAdjustment || Summary is not { } folio
                                     || AdjustmentComposer.Amount is not { } amount
                                     || AdjustmentComposer.SelectedType is not { } type) return;

            await SubmitAsync(
                new PostAdjustmentCommand(
                    Key,
                    amount,
                    folio.Currency,
                    type.Value,
                    AdjustmentComposer.Reason.Trim()),
                $"{folio.Currency} {amount:N2} {type.Label.ToLowerInvariant()} posted.",
                AdjustmentComposer.Reset);
        }

        private async Task SubmitRefundAsync()
        {
            if (!CanSubmitRefund || Summary is not { } folio
                                 || RefundComposer.Amount is not { } amount
                                 || RefundComposer.SelectedMethod is not { } method) return;

            string? reference = string.IsNullOrWhiteSpace(RefundComposer.Reference)
                                    ? null
                                    : RefundComposer.Reference.Trim();

            await SubmitAsync(
                new IssueRefundCommand(
                    Key,
                    amount,
                    folio.Currency,
                    method.Value,
                    reference,
                    RefundComposer.Reason.Trim()),
                $"{folio.Currency} {amount:N2} refunded ({method.Label.ToLowerInvariant()}).",
                RefundComposer.Reset);
        }

        // One path for all the forms: run the command, and only clear the form on success so a rejected
        // entry (for example, the folio was settled in the meantime) never loses what the user typed.
        private async Task SubmitAsync<TCommand>( TCommand command, string successMessage, Action resetForm ) where TCommand : ICommand
        {
            _isSubmitting = true;
            RaiseState();

            try
            {
                if (!await TryExecuteAsync(command)) return;

                _notifications.ShowSuccess(successMessage);
                resetForm();
                await ReloadAsync();
            }
            finally
            {
                _isSubmitting = false;
                RaiseState();
            }
        }

        // ── Settle / void ────────────────────────────────────────
        private async Task SettleAsync()
        {
            if (!CanSettle) return;

            bool confirmed = await _messageBoxService.ShowConfirmAsync(
                                 "Settle Folio",
                                 "Mark this folio as settled? Nothing more can be posted to it afterwards.");

            if (!confirmed) return;

            _isSubmitting = true;
            RaiseState();

            try
            {
                if (!await TryExecuteAsync(new SettleFolioCommand(Key))) return;

                _notifications.ShowSuccess("Folio settled.");
                _activeComposer = NoComposer;
                await ReloadAsync();
            }
            finally
            {
                _isSubmitting = false;
                RaiseState();
            }
        }

        private async Task VoidAsync()
        {
            if (!CanVoid || Summary is not { } folio) return;

            // The domain only blocks voiding a settled folio, so a folio with money on it can be voided.
            // Say so plainly before it happens.
            string message = folio.TotalCharges > 0 || folio.TotalPaid > 0
                                 ? $"This folio has {folio.ChargesDisplay} in charges and {folio.PaidDisplay} paid or credited. "
                                   + "Voiding it removes it from billing and cannot be undone. Only void a folio that was opened by mistake."
                                 : "Void this empty folio? This cannot be undone.";

            bool confirmed = await _messageBoxService.ShowConfirmAsync("Void Folio", message);

            if (!confirmed) return;

            _isSubmitting = true;
            RaiseState();

            try
            {
                if (!await TryExecuteAsync(new VoidFolioCommand(Key))) return;

                _notifications.ShowSuccess("Folio voided.");
                _activeComposer = NoComposer;
                await ReloadAsync();
            }
            finally
            {
                _isSubmitting = false;
                RaiseState();
            }
        }

        // ── Helpers ──────────────────────────────────────────────
        // ExecuteCommandAsync swallows exceptions (it shows the error) and returns default(TResult).
        // Unit is a reference type, so null means "it failed" and Unit.Value means "it worked".
        private async Task<bool> TryExecuteAsync<TCommand>( TCommand command ) where TCommand : ICommand =>
            await ExecuteCommandAsync<TCommand, Unit>(command) is not null;

        private async Task ReloadAsync()
        {
            try
            {
                await LoadAsync();
            }
            catch (Exception exception)
            {
                _notifications.ShowError($"Could not refresh the folio: {exception.Message}");
            }
        }

        private void OnComposerChanged( object? sender, EventArgs e ) => RaiseState();

        private void RaiseState()
        {
            foreach (string property in DerivedProperties)
                RaisePropertyChanged(property);

            SubmitChargeCommand.RaiseCanExecuteChanged();
            SubmitPaymentCommand.RaiseCanExecuteChanged();
            SubmitAdjustmentCommand.RaiseCanExecuteChanged();
            SubmitRefundCommand.RaiseCanExecuteChanged();
            SettleCommand.RaiseCanExecuteChanged();
            VoidCommand.RaiseCanExecuteChanged();
        }
    }
}
