using Innova.Application.RoomInventory.DTO;

namespace Innova.Presentation.Core.RoomInventory.Room.Lists
{
    /// <summary>
    ///     Lightweight display model for a single row in the room list.
    ///     This is not a full MvxViewModel. It contains no commands,
    ///     IoC dependencies, or asynchronous work.
    /// </summary>
    public sealed class RoomListItemViewModel
    {
        public RoomListItemViewModel( RoomDto dto )
        {
            RoomId = dto.RoomId;
            RoomNumber = dto.RoomNumber;
            RoomTypeName = dto.RoomTypeName;

            FloorLevel = dto.FloorLevel;
            FloorWing = dto.FloorWing;

            FloorDisplay = string.IsNullOrWhiteSpace(dto.FloorWing)
                               ? $"Floor {dto.FloorLevel}"
                               : $"Floor {dto.FloorLevel} · {dto.FloorWing}";

            RoomTypeId = dto.RoomTypeId;

            Status = dto.Status;

            StatusLabel = GetStatusLabel(dto.Status);
            StatusBadge = GetStatusBadge(dto.Status);
        }


        // ── Identity ────────────────────────────────────────────

        public Guid RoomId { get; }
        public string RoomNumber { get; }
        public string RoomTypeName { get; }

        // ── Location ────────────────────────────────────────────
        public int FloorLevel { get; }
        public string? FloorWing { get; }
        public string FloorDisplay { get; }

        // ── Room Type ────────────────────────────────────────────
        public Guid RoomTypeId { get; }

        // ── Status ───────────────────────────────────────────────
        public string Status { get; }
        public string StatusLabel { get; }
        public string StatusBadge { get; }

        // ── Helpers ──────────────────────────────────────────────
        private static string GetStatusLabel( string status )
        {
            return status switch
                   {
                       "Vacant"       => "Vacant",
                       "Occupied"     => "Occupied",
                       "Dirty"        => "Dirty",
                       "OutOfService" => "Out of Service",
                       _              => status
                   };
        }

        private static string GetStatusBadge( string status )
        {
            return status switch
                   {
                       "Vacant"       => "Success",
                       "Occupied"     => "Info",
                       "Dirty"        => "Warning",
                       "OutOfService" => "Danger",
                       _              => "Default"
                   };
        }
    }
}
