using Innova.Application.Dispatchers;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Services.Abstractions;

namespace Innova.Presentation.Core.Dashboard
{
    public sealed class DashboardViewModel:WorkspaceViewModel
    {
        public DashboardViewModel( CommandDispatcher commandDispatcher, IBusyIndicatorService busyIndicator, INotificationService notifications ):base(
            commandDispatcher,
            busyIndicator,
            notifications)
        {
        }
    }
}
