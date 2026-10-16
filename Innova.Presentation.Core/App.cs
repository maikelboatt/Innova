using Innova.Presentation.Core.Identity;
using MvvmCross.ViewModels;

namespace Innova.Presentation.Core
{
    public class App:MvxApplication
    {
        public override void Initialize()
        {
            RegisterAppStart<LoginViewModel>();
        }
    }
}
