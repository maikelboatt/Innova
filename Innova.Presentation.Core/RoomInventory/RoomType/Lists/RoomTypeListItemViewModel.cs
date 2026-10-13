using Innova.Application.RoomInventory.DTO;

namespace Innova.Presentation.Core.RoomInventory.RoomType.Lists
{
    public sealed class RoomTypeListItemViewModel
    {
        public RoomTypeListItemViewModel( RoomTypeDto dto )
        {
            RoomTypeId = dto.RoomTypeId;
            Name = dto.Name;

            MaxOccupancy = dto.MaxOccupancy;
            OccupancyDisplay = dto.MaxOccupancy == 1
                                   ? "1 guest"
                                   : $"{dto.MaxOccupancy} guests";

            BaseRateAmount = dto.BaseRateAmount;
            BaseRateCurrency = dto.BaseRateCurrency;
            BaseRateDisplay = $"{dto.BaseRateCurrency} {dto.BaseRateAmount:N2}";
        }

        // ── Identity ────────────────────────────────────────────
        public Guid RoomTypeId { get; }
        public string Name { get; }

        // ── Occupancy ──────────────────────────────────────────
        public int MaxOccupancy { get; }
        public string OccupancyDisplay { get; }

        // ── Rate ────────────────────────────────────────────────
        public decimal BaseRateAmount { get; }
        public string BaseRateCurrency { get; }
        public string BaseRateDisplay { get; }
    }
}
