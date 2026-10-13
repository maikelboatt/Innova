using System.Collections.ObjectModel;
using Innova.Application;
using Innova.Application.Dispatchers;
using Innova.Application.HouseKeeping.HouseKeeping.Commands.AssignTask;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.ViewModels;

namespace Innova.Presentation.Core.HouseKeeping.HouseKeeping.Forms
{
    public sealed class HouseKeepingAssignDialogViewModel:FormViewModel, IMvxViewModel<Guid>
    {
        private readonly IModalService _modalService;
        private Guid _staffId;
        private Guid _taskId;

        public HouseKeepingAssignDialogViewModel( CommandDispatcher commandDispatcher,
                                                  IBusyIndicatorService busyIndicator,
                                                  INotificationService notifications,
                                                  IMessageBoxService messageBox,
                                                  IModalService modalService ):base(
            commandDispatcher,
            busyIndicator,
            notifications,
            messageBox)
        {
            _modalService = modalService;
            IsSaveEnabled = false;
        }

        private ObservableCollection<StaffOption> ActiveStaffOptions { get; } = [];

        public Guid StaffId
        {
            get => _staffId;
            set
            {
                if (SetProperty(ref _staffId, value))
                {
                    MarkAsChanged();
                    RecalculateSaveEnabled();
                }
            }
        }

        public void Prepare( Guid parameter ) => _taskId = parameter;

        public override async Task Initialize()
        {
            await base.Initialize();
            await LoadActiveStaffAsync();
        }

        private async Task LoadActiveStaffAsync()
        {
            // Todo: Will query for all active staff one the aggregate has been written
            List<StaffDto> staff = [];

            ActiveStaffOptions.Clear();

            foreach (StaffDto staffOption in staff)
            {
                ActiveStaffOptions.Add(new StaffOption(staffOption.StaffId, $"{staffOption.FirstName} {staffOption.MiddleName} {staffOption.LastName}"));
            }
        }

        private void RecalculateSaveEnabled() => IsSaveEnabled = StaffId != Guid.Empty;

        protected override async Task OnSaveAsync()
        {
            Guid? taskId = await ExecuteCommandAsync<AssignTaskCommand, Guid>(new AssignTaskCommand(_taskId, _staffId));

            if (taskId is not { } id || id == Guid.Empty)
                return;

            HasUnsavedChanges = false;
            _modalService.Close();
        }

        protected override Task OnCancelConfirmedAsync()
        {
            _modalService.Close();
            return Task.CompletedTask;
        }
    }
}
