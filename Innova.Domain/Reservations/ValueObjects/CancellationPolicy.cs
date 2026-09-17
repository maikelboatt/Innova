using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Domain.Reservations.ValueObjects
{
    public sealed class CancellationPolicy:ValueObject
    {
        private CancellationPolicy( int freeCancellationWindowHours, Money feeIfWithinWindow )
        {
            FreeCancellationWindowHours = freeCancellationWindowHours;
            FeeIfWithinWindow = feeIfWithinWindow;
        }

        public int FreeCancellationWindowHours { get; }
        public Money FeeIfWithinWindow { get; }

        public static CancellationPolicy Of( int freeCancellationWindowHours, Money feeIfWithinWindow )
        {
            if (freeCancellationWindowHours < 0)
                throw new DomainException("Free cancellation window cannot be negative.");

            return new CancellationPolicy(freeCancellationWindowHours, feeIfWithinWindow);
        }

        // The rule lives here — one place — rather than being re-derived by
        // whatever calls it. Cancel() and any future handler both get the
        // same answer because there's only one method that can compute it.
        public Money FeeFor( DateTime cancelledAtUtc, DateOnly checkInDate )
        {
            double hoursUntilCheckIn = (checkInDate.ToDateTime(TimeOnly.MinValue) - cancelledAtUtc).TotalHours;

            return hoursUntilCheckIn >= FreeCancellationWindowHours
                       ? Money.Zero(FeeIfWithinWindow.Currency)
                       : FeeIfWithinWindow;
        }

        public override string ToString() => $"Free up to {FreeCancellationWindowHours}h before check-in, then {FeeIfWithinWindow}";

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return FreeCancellationWindowHours;
            yield return FeeIfWithinWindow;
        }
    }
}
