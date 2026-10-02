-- Update.sql Reservations

UPDATE reservations.GroupBooking
SET OrganizerGuestId = @OrganizerGuestId,
    GroupName        = @GroupName
WHERE Id = @Id;
