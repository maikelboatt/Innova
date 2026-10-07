using Innova.Presentation.Core.Shell;
using MvvmCross.ViewModels;

namespace Innova.Presentation.Core
{
    public class App:MvxApplication
    {
        public override void Initialize()
        {
            RegisterAppStart<ShellViewModel>();
        }
    }
}
