using Innova.Presentation.Core.Services.Abstractions;

namespace Innova.Presentation.Core.Services
{
    /// <summary>
    ///     Ref-counted so concurrent Show/Hide calls compose correctly
    ///     instead of one caller hiding the indicator while another still
    ///     expects it visible. This directly matters for the race flagged
    ///     earlier between PatientListViewModel's post-modal refresh and
    ///     EditPatientViewModel.Initialize() — both can legitimately call
    ///     Show()/Hide() in an overlapping window, and a plain bool flag
    ///     would let the second Hide() clear it while the first caller's
    ///     work is still in flight.
    /// </summary>
    public sealed class BusyIndicatorService:IBusyIndicatorService
    {
        private readonly object _lock = new();
        private int _busyCount;

        // Not part of IBusyIndicatorService — ViewModels only need Show/Hide.
        // Exposed here so a global busy overlay in ShellView can display
        // the current message, if you want one. If ShellViewModel needs
        // this, it resolves BusyIndicatorService's concrete type (or the
        // interface grows a Message member later) rather than every
        // ViewModel gaining access to it through the abstraction.
        public string Message { get; private set; } = "Loading...";

        public bool IsBusy { get; private set; }

        public event EventHandler<bool>? BusyStateChanged;

        public void Show( string message = "Loading..." )
        {
            bool justBecameBusy = false;

            lock (_lock)
            {
                _busyCount++;
                Message = message;

                if (!IsBusy)
                {
                    IsBusy = true;
                    justBecameBusy = true;
                }
            }

            if (justBecameBusy)
                BusyStateChanged?.Invoke(this, true);
        }

        public void Hide()
        {
            bool justBecameIdle = false;

            lock (_lock)
            {
                if (_busyCount > 0)
                    _busyCount--;

                if (_busyCount == 0 && IsBusy)
                {
                    IsBusy = false;
                    justBecameIdle = true;
                }
            }

            if (justBecameIdle)
                BusyStateChanged?.Invoke(this, false);
        }
    }
}
