using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.Shared.ValueObjects
{
    public sealed class DateRange:ValueObject
    {
        private DateRange( DateOnly start, DateOnly end )
        {
            Start = start;
            End = end;
        }

        public DateOnly Start { get; }
        public DateOnly End { get; }

        public static DateRange Of( DateOnly start, DateOnly end )
        {
            if (end <= start) throw new DomainException("End date must be after start date (minimum one night).");
            return new DateRange(start, end);
        }

        public bool Overlaps( DateRange other ) => Start < other.End && other.Start < End;

        public bool Contains( DateOnly date ) => date >= Start && date < End;

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Start;
            yield return End;
        }
    }
}
