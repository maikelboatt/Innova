using Innova.Presentation.Core.Services;
using Innova.Presentation.Core.Services.Abstractions;
using Innova.Presentation.WPF.Services;
using MvvmCross.Base;
using MvvmCross.IoC;

namespace Innova.Presentation.WPF
{
    // ══════════════════════════════════════════════════════════════════
    // PresentationCompositionRoot — registers every Core (Presentation)
    // service abstraction. This is the ONLY place in the solution
    // allowed to reference both Infrastructure's CompositionRoot seam
    // and Presentation.Core's concrete service implementations —
    // Presentation.Wpf already legitimately depends on both.
    // ══════════════════════════════════════════════════════════════════
    public static class PresentationCompositionRoot
    {
        public static void RegisterPresentationServices( IMvxIoCProvider iocProvider )
        {

            // IMvxMainThreadAsyncDispatcher is registered by MvvmCross itself
            // during platform setup — resolved here, not constructed.
            IMvxMainThreadAsyncDispatcher mainThreadDispatcher =
                iocProvider.Resolve<IMvxMainThreadAsyncDispatcher>();

            // Stateless / single global instance — bound to by ShellViewModel
            iocProvider.RegisterSingleton<IBusyIndicatorService>(new BusyIndicatorService());

            NotificationService notificationService = new(mainThreadDispatcher);
            iocProvider.RegisterSingleton<INotificationService>(notificationService);
            iocProvider.RegisterSingleton<INotificationFeed>(notificationService);

            // ModalService needs the container itself to resolve modal
            // ViewModels on demand — constructed explicitly rather than
            // via RegisterType so its IMvxIoCProvider dependency is
            // unambiguous.
            iocProvider.RegisterSingleton<IModalService>(new ModalService(iocProvider));
            iocProvider.RegisterSingleton<IContentService>(new ContentService(iocProvider));

            iocProvider.RegisterType<IMessageBoxService, MessageBoxService>();
            // iocProvider.RegisterType<IPdfGenerator, QuestPdfGenerator>();
            iocProvider.RegisterType<IFileSaveService, FileSaveService>();
        }
    }
}
