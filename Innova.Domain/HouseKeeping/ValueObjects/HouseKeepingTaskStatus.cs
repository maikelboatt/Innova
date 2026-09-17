using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.HouseKeeping.ValueObjects
{
    public sealed class HouseKeepingTaskStatus:ValueObject
    {
        public static readonly HouseKeepingTaskStatus Pending = new("Pending");
        public static readonly HouseKeepingTaskStatus InProgress = new("InProgress");
        public static readonly HouseKeepingTaskStatus Completed = new("Completed");
        public static readonly HouseKeepingTaskStatus Failed = new("Failed");

        private HouseKeepingTaskStatus( string value ) => Value = value;

        public string Value { get; }

        public bool CanTransitionTo( HouseKeepingTaskStatus next )
        {
            return Value switch
                   {
                       "Pending" => next == InProgress,

                       "InProgress" => next == Completed || next == Failed,

                       _ => false
                   };
        }

        public static HouseKeepingTaskStatus FromValue( string value ) => value switch
                                                                          {
                                                                              "Pending" => Pending,
                                                                              "InProgress" => InProgress,
                                                                              "Completed" => Completed,
                                                                              "Failed" => Failed,
                                                                              _ => throw new DomainException($"'{value}' is not a valid House keeping status")
                                                                          };

        public override string ToString() => Value;

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
