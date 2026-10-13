using Innova.Application.Billing.Folio.Commands.OpenFolio;
using Innova.Application.Billing.Folio.DTO;
using Innova.Application.Billing.Folio.Queries;
using Innova.Application.Dispatchers;
using Innova.Application.Settings;
using Innova.Domain.Billing.ValueObjects;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
// adjust if your OpenFolioCommand lives in Innova.Application.Billing.Commands

namespace Innova.Presentation.Core.Billing.Folio.Forms
{
    /// <summary>
    ///     Modal form that opens a folio. The user picks what the folio is for (a room stay, a guest in a stay,
    ///     a group or a guest), then which one, then confirms the currency (defaults to the hotel currency).
    ///     The "which one" list is loaded per owner type through ListFolioOwnerOptionsQuery.
    /// </summary>
    public sealed class OpenFolioViewModel:ViewModelBase, IMvxViewModel<OpenFolioArgs>
    {
        private static readonly string[] DerivedProperties =
        [
            nameof(OwnerPickerTitle), nameof(ShowEmptyOwners), nameof(EmptyOwnersMessage),
            nameof(DuplicateWarning), nameof(HasDuplicateWarning),
            nameof(CurrencyWarning), nameof(HasCurrencyWarning),
            nameof(ValidationMessage), nameof(HasValidationMessage), nameof(CanSave)
        ];

        private readonly HotelSettings _hotelSettings;
        private readonly IModalService _modalService;
        private readonly INotificationService _notifications;
        private readonly QueryDispatcher _queryDispatcher;

        private IReadOnlyList<FolioOwnerOptionViewModel> _allOwners = [];
        private OpenFolioArgs _args = new();
        private string _currency = string.Empty;
        private bool _isLoadingOwners;
        private bool _isSaving;
        private int _loadVersion;
        private string _ownerSearchText = string.Empty;
        private IReadOnlyList<FolioOwnerOptionViewModel> _owners = [];

        public OpenFolioViewModel( CommandDispatcher commandDispatcher,
                                   IBusyIndicatorService busyIndicator,
                                   INotificationService notifications,
                                   IModalService modalService,
                                   QueryDispatcher queryDispatcher,
                                   HotelSettings hotelSettings ):base(
            commandDispatcher,
            busyIndicator,
            notifications)
        {
            _notifications = notifications;
            _modalService = modalService;
            _queryDispatcher = queryDispatcher;
            _hotelSettings = hotelSettings;

            OwnerTypes =
            [
                new FolioOwnerTypeOptionViewModel(
                    FolioOwnerType.Stay,
                    "Room stay",
                    "Room charges and everything billed to the room.",
                    "stay"),
                new FolioOwnerTypeOptionViewModel(
                    FolioOwnerType.GuestWithinStay,
                    "Guest in a stay",
                    "A separate bill for one guest in a room (split billing).",
                    "guest"),
                new FolioOwnerTypeOptionViewModel(
                    FolioOwnerType.GroupBooking,
                    "Group booking",
                    "A master bill for a whole group.",
                    "group"),
                new FolioOwnerTypeOptionViewModel(
                    FolioOwnerType.Guest,
                    "Guest",
                    "Fees outside a stay, such as cancellation or no-show fees.",
                    "guest")
            ];

            SelectOwnerTypeCommand = new MvxAsyncCommand<FolioOwnerTypeOptionViewModel>(SelectOwnerTypeAsync);
            SelectOwnerCommand = new MvxCommand<FolioOwnerOptionViewModel>(SelectOwner);
            SaveCommand = new MvxAsyncCommand(SaveAsync, () => CanSave);
            CancelCommand = new MvxCommand(() => _modalService.Close());
        }

        // ── Commands ─────────────────────────────────────────────
        public IMvxAsyncCommand<FolioOwnerTypeOptionViewModel> SelectOwnerTypeCommand { get; }
        public IMvxCommand<FolioOwnerOptionViewModel> SelectOwnerCommand { get; }
        public IMvxAsyncCommand SaveCommand { get; }
        public IMvxCommand CancelCommand { get; }

        // ── Step 1: what is the folio for ────────────────────────
        public IReadOnlyList<FolioOwnerTypeOptionViewModel> OwnerTypes { get; }

        public FolioOwnerTypeOptionViewModel? SelectedOwnerType { get; private set; }

        /// <summary>Heading of the picker, e.g. "Choose the stay".</summary>
        public string OwnerPickerTitle => SelectedOwnerType is { } type
                                              ? $"Choose the {type.OwnerNoun}"
                                              : "Choose an owner";

        // ── Step 2: which one ────────────────────────────────────
        public IReadOnlyList<FolioOwnerOptionViewModel> Owners
        {
            get => _owners;
            private set
            {
                if (SetProperty(ref _owners, value))
                    RaiseDerived();
            }
        }

        public string OwnerSearchText
        {
            get => _ownerSearchText;
            set
            {
                if (!SetProperty(ref _ownerSearchText, value)) return;
                ApplyOwnerFilter();
            }
        }

        public bool IsLoadingOwners
        {
            get => _isLoadingOwners;
            private set
            {
                if (SetProperty(ref _isLoadingOwners, value))
                    RaiseDerived();
            }
        }

        public FolioOwnerOptionViewModel? SelectedOwner { get; private set; }

        public bool ShowEmptyOwners => !IsLoadingOwners && Owners.Count == 0;

        public string EmptyOwnersMessage
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_ownerSearchText) && _allOwners.Count > 0)
                    return "Nothing matches your search.";

                return SelectedOwnerType?.Type switch
                       {
                           FolioOwnerType.Stay            => "No guests are checked in right now.",
                           FolioOwnerType.GuestWithinStay => "No guests are checked in right now.",
                           FolioOwnerType.GroupBooking    => "There are no group bookings yet.",
                           FolioOwnerType.Guest           => "There are no active guests yet.",
                           _                              => string.Empty
                       };
            }
        }

        /// <summary>Non-blocking: the owner already has an open folio (opening another is allowed, e.g. to split a bill).</summary>
        public string? DuplicateWarning => SelectedOwner is { HasOpenFolios: true } owner
                                               ? $"{owner.Label} already has {owner.OpenFoliosDisplay}. You can still open another one, for example to split a bill."
                                               : null;

        public bool HasDuplicateWarning => DuplicateWarning is not null;

        // ── Step 3: currency ─────────────────────────────────────
        public string Currency
        {
            get => _currency;
            set
            {
                if (SetProperty(ref _currency, value))
                    RaiseDerived();
            }
        }

        /// <summary>Non-blocking: other parts of the system post charges in the hotel currency.</summary>
        public string? CurrencyWarning => IsValidCurrency && !string.Equals(Currency, _hotelSettings.DefaultCurrency, StringComparison.OrdinalIgnoreCase)
                                              ? $"Room charges are posted in {_hotelSettings.DefaultCurrency}. A {Currency.ToUpperInvariant()} folio cannot receive them."
                                              : null;

        public bool HasCurrencyWarning => CurrencyWarning is not null;

        // ── Validation ───────────────────────────────────────────
        public string? ValidationMessage
        {
            get
            {
                if (SelectedOwnerType is not { } type) return "Choose what the folio is for.";
                if (SelectedOwner is null) return $"Choose the {type.OwnerNoun}.";
                if (!IsValidCurrency) return "Enter a 3-letter currency code, for example GHS.";
                return null;
            }
        }

        public bool HasValidationMessage => ValidationMessage is not null;
        public bool CanSave => ValidationMessage is null && !_isSaving;

        private bool IsValidCurrency => _currency.Trim()
                                                 .Length == 3 && _currency
                                                                 .Trim()
                                                                 .All(char.IsLetter);

        public void Prepare( OpenFolioArgs parameter ) => _args = parameter;

        // ── Lifecycle ────────────────────────────────────────────
        public override async Task Initialize()
        {
            await base.Initialize();

            Currency = _hotelSettings.DefaultCurrency;

            // Most folios at the front desk belong to a room stay.
            FolioOwnerTypeOptionViewModel initial = OwnerTypes.FirstOrDefault(option => option.Type == _args.OwnerType)
                                                    ?? OwnerTypes[0];

            await SelectOwnerTypeAsync(initial);
        }

        // ── Actions ──────────────────────────────────────────────
        private async Task SelectOwnerTypeAsync( FolioOwnerTypeOptionViewModel? option )
        {
            if (option is null || ReferenceEquals(option, SelectedOwnerType)) return;

            foreach (FolioOwnerTypeOptionViewModel candidate in OwnerTypes)
                candidate.IsSelected = ReferenceEquals(candidate, option);

            SelectedOwnerType = option;
            SelectedOwner = null;
            _ownerSearchText = string.Empty;
            RaisePropertyChanged(nameof(SelectedOwnerType));
            RaisePropertyChanged(nameof(OwnerSearchText));
            RaisePropertyChanged(nameof(SelectedOwner));
            RaiseDerived();

            await LoadOwnersAsync(option);
        }

        private async Task LoadOwnersAsync( FolioOwnerTypeOptionViewModel type )
        {
            // A fast second click must not let the first (slower) response overwrite the list.
            int version = ++_loadVersion;

            _allOwners = [];
            Owners = [];
            IsLoadingOwners = true;

            try
            {
                IReadOnlyCollection<FolioOwnerOptionDto> dtos =
                    await _queryDispatcher.DispatchAsync(new ListFolioOwnerOptionsQuery(type.Type.ToString()));

                if (version != _loadVersion) return;

                _allOwners = [.. dtos.Select(dto => new FolioOwnerOptionViewModel(dto))];
                ApplyOwnerFilter();

                // Pre-selection (opened from a stay or guest screen): apply it once, then forget it.
                if (_args.OwnerId is { } ownerId && _args.OwnerType == type.Type)
                {
                    SelectOwner(_allOwners.FirstOrDefault(owner => owner.OwnerId == ownerId));
                    _args = _args with
                            {
                                OwnerId = null
                            };
                }
            }
            catch (Exception exception)
            {
                if (version == _loadVersion)
                    _notifications.ShowError($"Could not load the list: {exception.Message}");
            }
            finally
            {
                if (version == _loadVersion)
                    IsLoadingOwners = false;
            }
        }

        private void SelectOwner( FolioOwnerOptionViewModel? option )
        {
            foreach (FolioOwnerOptionViewModel candidate in _allOwners)
                candidate.IsSelected = ReferenceEquals(candidate, option);

            SelectedOwner = option;
            RaisePropertyChanged(nameof(SelectedOwner));
            RaiseDerived();
        }

        private void ApplyOwnerFilter()
        {
            string text = _ownerSearchText.Trim();

            Owners = text.Length == 0
                         ? _allOwners
                         :
                         [
                             .. _allOwners.Where(owner => owner.Label.Contains(text, StringComparison.OrdinalIgnoreCase)
                                                          || owner.Detail.Contains(text, StringComparison.OrdinalIgnoreCase))
                         ];
        }

        private async Task SaveAsync()
        {
            if (!CanSave || SelectedOwnerType is not { } type || SelectedOwner is not { } owner) return;

            _isSaving = true;
            RaiseDerived();

            try
            {
                OpenFolioCommand command = new(
                    type.Type,
                    owner.OwnerId,
                    _currency
                        .Trim()
                        .ToUpperInvariant());

                Guid folioId = await ExecuteCommandAsync<OpenFolioCommand, Guid>(command);

                // ExecuteCommandAsync has already surfaced the error. Stay open so the choices are not lost.
                if (folioId == Guid.Empty) return;

                _notifications.ShowSuccess($"Folio opened for {owner.Label}.");
                _modalService.Close();
            }
            finally
            {
                _isSaving = false;
                RaiseDerived();
            }
        }

        // ── Helpers ──────────────────────────────────────────────
        private void RaiseDerived()
        {
            foreach (string property in DerivedProperties)
                RaisePropertyChanged(property);

            SaveCommand.RaiseCanExecuteChanged();
        }
    }
}
