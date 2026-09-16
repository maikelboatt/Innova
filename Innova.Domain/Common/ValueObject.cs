namespace Innova.Domain.Common
{
    public abstract class ValueObject
    {
        protected abstract IEnumerable<object?> GetEqualityComponents();

        public override bool Equals( object? obj )
        {
            if (obj is null || obj.GetType() != GetType()) return false;
            return ((ValueObject)obj)
                   .GetEqualityComponents()
                   .SequenceEqual(GetEqualityComponents());
        }

        public override int GetHashCode() => GetEqualityComponents()
            .Aggregate(
                0,
                ( hash, component ) =>
                    HashCode.Combine(hash, component?.GetHashCode() ?? 0));

        public static bool operator ==( ValueObject? a, ValueObject? b ) => a is null && b is null || a is not null && b is not null && a.Equals(b);

        public static bool operator !=( ValueObject? a, ValueObject? b ) => !(a == b);
    }
}
