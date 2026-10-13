using Innova.Application.Dispatchers;
using Innova.Application.HouseKeeping.HouseKeeping.Commands.CompleteTask;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.ViewModels;

namespace Innova.Presentation.Core.HouseKeeping.HouseKeeping.Forms
{
    public sealed class HouseKeepingCompleteDialogViewModel:FormViewModel, IMvxViewModel<Guid>
    {
        private readonly IModalService _modalService;
        private string? _inspectionNotes;
        private bool _inspectionPassed;
        private Guid _taskId;

        public HouseKeepingCompleteDialogViewModel( CommandDispatcher commandDispatcher,
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

        public string? InspectionNotes
        {
            get => _inspectionNotes;
            set
            {
                if (SetProperty(ref _inspectionNotes, value))
                {
                    MarkAsChanged();
                    RecalculateSaveEnabled();
                }
            }
        }
        public bool InspectionPassed
        {
            get => _inspectionPassed;
            set
            {
                if (SetProperty(ref _inspectionPassed, value))
                {
                    MarkAsChanged();
                    RecalculateSaveEnabled();
                }
            }
        }

        public void Prepare( Guid parameter )
        {
            _taskId = parameter;
        }

        private void RecalculateSaveEnabled()
        {
            IsSaveEnabled = InspectionPassed && InspectionNotes is null;
        }

        protected override async Task OnSaveAsync()
        {
            await ExecuteCommandAsync(new CompleteTaskCommand(_taskId, InspectionPassed, InspectionNotes));

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
