-- Insert.sql - FrontDesk

INSERT INTO frontdesk.Stay
(Id,
 ReservationId,
 GroupBookingId,
 AssignedRoomId,
 AssignedRoomNumber,
 MaxOccupancy,
 ActualCheckIn,
 ActualCheckOut)
VALUES (@Id,
        @ReservationId,
        @GroupBookingId,
        @AssignedRoomId,
        @AssignedRoomNumber,
        @MaxOccupancy,
        @ActualCheckIn,
        @ActualCheckOut);