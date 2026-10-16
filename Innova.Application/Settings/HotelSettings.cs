namespace Innova.Application.Settings
{
    public sealed class HotelSettings
    {
        public const string SectionName = "Hotel";

        public string DefaultCurrency { get; init; } = "GHS";

        public string DefaultCountry { get; init; } = "GH";

        public TimeOnly CheckInTime { get; init; } = new(14, 0);

        public TimeOnly CheckOutTime { get; init; } = new(12, 0);

        public int CancellationWindowHours { get; init; } = 24;

        public decimal CancellationFeeAmount { get; init; } = 50m;
    }
}
