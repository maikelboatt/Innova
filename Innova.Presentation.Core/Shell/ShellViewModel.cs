using Innova.Application.Dispatchers;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Services.Abstractions;

namespace Innova.Presentation.Core.Shell
{
    public class ShellViewModel:ViewModelBase
    {
        public ShellViewModel( CommandDispatcher commandDispatcher, IBusyIndicatorService busyIndicator, INotificationService notifications ):base(
            commandDispatcher,
            busyIndicator,
            notifications)
        {
        }
    }
}
