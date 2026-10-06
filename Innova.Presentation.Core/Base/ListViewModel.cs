// MediCore.Presentation.Core/Base/ListViewModel.cs

using System.Collections.ObjectModel;
using Innova.Application.Dispatchers;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.Commands;

namespace Innova.Presentation.Core.Base
{
    /// <summary>
    ///     Base class for ViewModels that display a searchable,
    ///     selectable list of items.
    ///     Examples: GuestListViewModel, ReservationListViewModel,
    ///     RoomListViewModel, BillListViewModel
    ///     Adds on top of WorkspaceViewModel:
    ///     - ObservableCollection
    ///     <TItem>
    ///         Items
    ///         - TItem? SelectedItem with SelectCommand
    ///         - SearchText with client-side filtering via FilterItems()
    ///         - HasItems / IsEmpty computed properties for empty-state UI
    /// </summary>
    public abstract class ListViewModel<TItem>:WorkspaceViewModel
        where TItem : class
    {
        private ObservableCollection<TItem> _items = [];
        private string _searchText = string.Empty;
        private TItem? _selectedItem;

        protected ListViewModel( CommandDispatcher commandDispatcher, IBusyIndicatorService busyIndicator, INotificationService notifications )
            :base(commandDispatcher, busyIndicator, notifications) => SelectCommand = new MvxAsyncCommand<TItem>(OnItemSelectedAsync);

        // ── Items ─────────────────────────────────────────────────────
        // The full displayed collection. Bind this to a DataGrid or
        // ItemsControl in the View.
        public ObservableCollection<TItem> Items
        {
            get => _items;
            protected set => SetProperty(ref _items, value);
        }

        // ── SelectedItem ──────────────────────────────────────────────
        // The currently selected row/card. Subclasses react to this
        // changing by navigating to a detail view via IContentService.
        public TItem? SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        // ── SearchText ────────────────────────────────────────────────
        // Bound to a search box. When changed, calls FilterItems()
        // so subclasses define their own filter logic.
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    ApplyFilter();
            }
        }

        // ── HasItems / IsEmpty ────────────────────────────────────────
        // Bind IsEmpty to an empty-state panel ("No rooms found")
        // and HasItems to the list itself for Visibility binding.
        public bool HasItems => Items.Count > 0;
        public bool IsEmpty => Items.Count == 0;

        // ── SelectCommand ─────────────────────────────────────────────
        // Bind to ItemsControl.InputBindings or a button in each row.
        // Calls OnItemSelected which subclasses override.
        public IMvxAsyncCommand<TItem> SelectCommand { get; }

        // ══════════════════════════════════════════════════════════════
        // SetItems — call this from LoadAsync() after fetching data.
        // Updates Items and notifies HasItems/IsEmpty.
        // ══════════════════════════════════════════════════════════════
        protected void SetItems( IEnumerable<TItem> items )
        {
            Items.Clear();

            foreach (TItem item in items)
                Items.Add(item);
            // Items = new ObservableCollection<TItem>(items);
            RaisePropertyChanged(nameof(HasItems));
            RaisePropertyChanged(nameof(IsEmpty));
        }

        // ══════════════════════════════════════════════════════════════
        // FilterItems — override to apply SearchText filtering.
        // Called automatically when SearchText changes.
        //
        // Example override:
        //   protected override void FilterItems()
        //   {
        //       var filtered = _allGuests
        //           .Where(p => p.FullName.Contains(SearchText,
        //               StringComparison.OrdinalIgnoreCase));
        //       SetItems(filtered);
        //   }
        // ══════════════════════════════════════════════════════════════
        protected virtual void ApplyFilter() { }

        // ══════════════════════════════════════════════════════════════
        // OnItemSelected — override to handle row selection.
        // Typically navigates to a detail view:
        //
        //   protected override void OnItemSelected(GuestListItemViewModel item)
        //   {
        //       _contentService.NavigateTo<GuestDetailsViewModel>(item.GuestId);
        //   }
        // ══════════════════════════════════════════════════════════════
        protected virtual Task OnItemSelectedAsync( TItem item )
        {
            SelectedItem = item;
            return Task.CompletedTask;
        }
    }
}
