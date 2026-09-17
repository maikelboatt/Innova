using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.HouseKeeping.ValueObjects
{
    public sealed class InspectionResult:ValueObject
    {
        private InspectionResult( bool passed, string? notes )
        {
            Passed = passed;
            Notes = notes;
        }

        public bool Passed { get; }

        public string? Notes { get; }

        public static InspectionResult Of( bool passed, string? notes = null )
        {
            return passed switch
                   {
                       true when !string.IsNullOrWhiteSpace(notes) => throw new DomainException("A passed inspection should not contain failure notes."),
                       false when string.IsNullOrWhiteSpace(notes) => throw new DomainException("Notes are required when an inspection fails."),
                       _ => new InspectionResult(
                           passed,
                           string.IsNullOrWhiteSpace(notes)
                               ? null
                               : notes.Trim())
                   };

        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Passed;
            yield return Notes;
        }

        public override string ToString() => Passed
                                                 ? "Passed"
                                                 : $"Failed: {Notes}";
    }
}
