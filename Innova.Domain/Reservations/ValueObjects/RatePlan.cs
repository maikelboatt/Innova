using Innova.Domain.Common;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Domain.Reservations.ValueObjects
{
    public sealed class RatePlan:ValueObject
    {
        private RatePlan( Money nightlyRate, CancellationPolicy cancellationPolicy )
        {
            NightlyRate = nightlyRate;
            CancellationPolicy = cancellationPolicy;
        }

        public Money NightlyRate { get; }
        public CancellationPolicy CancellationPolicy { get; }

        public static RatePlan Of( Money nightlyRate, CancellationPolicy cancellationPolicy ) => new(nightlyRate, cancellationPolicy);

        public override string ToString() => $"Rate: '{NightlyRate}', Cancellation policy: '{CancellationPolicy}'";

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return NightlyRate;
            yield return CancellationPolicy;
        }
    }
}
