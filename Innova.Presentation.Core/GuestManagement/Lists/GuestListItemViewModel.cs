using Innova.Application.GuestManagement.Guest.DTO;

namespace Innova.Presentation.Core.GuestManagement.Lists
{
    /// <summary>
    ///     Ligthweight display model for a single row in the guest list.
    ///     This is NOT a full MvxViewModel — it has no commands, no IoC
    ///     dependencies, and no async work. It is a pure display object
    ///     constructed from a GuestDto by GuestListViewModel.
    ///     The DataGrid binds its columns directly to these properties.
    /// </summary>
    public sealed class GuestListItemViewModel
    {
        public GuestListItemViewModel( GuestDto dto )
        {

            GuestId = dto.GuestId;
            FullName = $"{dto.FirstName} {dto.LastName}";
            FirstName = dto.FirstName;
            LastName = dto.LastName;
            DateOfBirth = dto.DateOfBirth;
            Age = CalculateAge(dto.DateOfBirth);
            PhoneNumber = dto.PhoneNumber;
            IdentityDocumentType = dto.IdentityDocumentType;
            IdentityDocumentNumber = dto.IdentityDocumentNumber;
            Email = dto.Email ?? "-";
            IsActive = dto.IsActive;

            StatusLabel = IsActive
                              ? "Active"
                              : "Inactive";
            StatusBadge = IsActive
                              ? "Success"
                              : "Danger";

            RegisteredAt = dto.CreatedAt;
            RegisteredDisplay = dto.CreatedAt.ToString("dd MMM yyyy");
        }

        public Guid GuestId { get; }
        public string FullName { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public DateOnly DateOfBirth { get; }
        public int Age { get; }
        public string PhoneNumber { get; }
        public string IdentityDocumentType { get; }
        public string IdentityDocumentNumber { get; }
        public string? Email { get; }
        public bool IsActive { get; }

        // Status badge
        public string StatusLabel { get; }
        public string StatusBadge { get; }

        // Registration
        public DateTime RegisteredAt { get; }
        public string RegisteredDisplay { get; }


        // ── Helpers ───────────────────────────────────────────────
        private int CalculateAge( DateOnly dtoDateOfBirth )
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            int age = today.Year - dtoDateOfBirth.Year;

            if (today < dtoDateOfBirth.AddYears(age))
                age--;

            return age;
        }
    }
}
