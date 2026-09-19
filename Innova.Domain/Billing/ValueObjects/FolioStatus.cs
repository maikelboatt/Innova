using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.Billing.ValueObjects
{
    public sealed class FolioStatus:ValueObject
    {
        public static readonly FolioStatus Open = new("Open");
        public static readonly FolioStatus Settled = new("Settled");
        public static readonly FolioStatus Void = new("Void");

        private FolioStatus( string value ) => Value = value;

        public string Value { get; }

        public static FolioStatus FromValue( string value ) => value switch
                                                               {
                                                                   "Open"    => Open,
                                                                   "Settled" => Settled,
                                                                   "Void"    => Void,
                                                                   _         => throw new DomainException($"'{value}' is not a valid payment status")
                                                               };

        public bool CanTransitionTo( FolioStatus next ) => Value switch
                                                           {
                                                               "Open" => next == Settled || next == Void,
                                                               _      => false
                                                           };

        public override string ToString() => Value;

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
