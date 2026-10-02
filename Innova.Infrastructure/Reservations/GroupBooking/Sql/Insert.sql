-- Insert.sql  Reservations

INSERT INTO reservations.GroupBooking
(Id,
 OrganizerGuestId,
 GroupName)
VALUES (@Id,
        @OrganizerGuestId,
        @GroupName);