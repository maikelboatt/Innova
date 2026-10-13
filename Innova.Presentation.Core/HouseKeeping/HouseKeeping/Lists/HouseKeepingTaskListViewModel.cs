using Innova.Application.Dispatchers;
using Innova.Application.HouseKeeping.HouseKeeping.DTO;
using Innova.Application.HouseKeeping.HouseKeeping.Queries;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.Commands;

namespace Innova.Presentation.Core.HouseKeeping.HouseKeeping.Lists
{
    public sealed class HouseKeepingTaskListViewModel:ListViewModel<HouseKeepingTaskListItemViewModel>
    {
        private readonly IModalService _modalService;
        private readonly QueryDispatcher _queryDispatcher;

        private IReadOnlyList<HouseKeepingTaskListItemViewModel> _allTasks = [];

        public HouseKeepingTaskListViewModel(
            CommandDispatcher commandDispatcher,
            IBusyIndicatorService busyIndicator,
            INotificationService notifications,
            QueryDispatcher queryDispatcher,
            IModalService modalService )
            :base(commandDispatcher, busyIndicator, notifications)
        {
            _queryDispatcher = queryDispatcher;
            _modalService = modalService;

            Title = "Housekeeping";

            OpenTaskDetailsCommand = new MvxAsyncCommand<Guid>(OnOpenTaskDetailsAsync);
            UpdateTaskCommand = new MvxAsyncCommand<Guid>(OnUpdateTaskAsync);
        }

        public IMvxAsyncCommand<Guid> OpenTaskDetailsCommand { get; }
        public IMvxAsyncCommand<Guid> UpdateTaskCommand { get; }

        protected override async Task LoadAsync()
        {
            IReadOnlyCollection<HouseKeepingTaskSummaryDto> dtos =
                await _queryDispatcher.DispatchAsync(
                    new ListHouseKeepingTasksQuery(
                        null,
                        null,
                        null));

            _allTasks =
            [
                .. dtos.Select(dto =>
                                   new HouseKeepingTaskListItemViewModel(dto))
            ];
        }

        protected override void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                SetItems(_allTasks);
                return;
            }

            string searchText = SearchText.Trim();

            IEnumerable<HouseKeepingTaskListItemViewModel> filtered =
                _allTasks.Where(task =>
                                    task.TypeLabel.Contains(
                                        searchText,
                                        StringComparison.OrdinalIgnoreCase)
                                    || task.StatusLabel.Contains(
                                        searchText,
                                        StringComparison.OrdinalIgnoreCase)
                                    || task.AssignmentLabel.Contains(
                                        searchText,
                                        StringComparison.OrdinalIgnoreCase));

            SetItems(filtered);
        }

        private async Task OnOpenTaskDetailsAsync( Guid taskId )
        {
            // await _modalService
            //     .ShowAsync<HouseKeepingTaskDetailsViewModel, Guid>(
            //         taskId);

            await RefreshCommand.ExecuteAsync();
        }

        private async Task OnUpdateTaskAsync( Guid taskId )
        {
            // await _modalService
            //     .ShowAsync<HouseKeepingTaskUpdateViewModel, Guid>(
            //         taskId);

            await RefreshCommand.ExecuteAsync();
        }

        protected override async Task OnItemSelectedAsync(
            HouseKeepingTaskListItemViewModel item )
        {
            await base.OnItemSelectedAsync(item);

            // await _modalService
            //     .ShowAsync<HouseKeepingTaskDetailsViewModel, Guid>(
            //         item.TaskId);

            await RefreshCommand.ExecuteAsync();
        }
    }
}
