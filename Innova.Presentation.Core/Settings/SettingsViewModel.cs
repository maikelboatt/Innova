using Innova.Application.Dispatchers;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Services.Abstractions;

namespace Innova.Presentation.Core.Settings
{
    public sealed class SettingsViewModel:WorkspaceViewModel
    {
        public SettingsViewModel( CommandDispatcher commandDispatcher, IBusyIndicatorService busyIndicator, INotificationService notifications ):base(
            commandDispatcher,
            busyIndicator,
            notifications)
        {
        }
    }
}
