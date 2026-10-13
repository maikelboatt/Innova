-- 0013_SeedDemoData.sql
-- ============================================================================================
-- DEMO / DEVELOPMENT DATA ONLY. Do not apply this file to a production database.
--
--   * Fills every table created by migrations 0001, 0003-0010: guests, room types, rooms,
--     per-night allotments, group bookings, reservations, stays + occupants, housekeeping tasks,
--     and billing (folios, charges, payments, adjustments).
--   * Not seeded on purpose: auth.[User] (no credentials in a migration - create the first admin
--     from configuration at startup) and audit.AuditLog (written by the application).
--   * Safe to run twice: it does nothing if guests or room types already exist.
--   * All dates are relative to the day the script runs, so the data always looks "current".
--   * Everything is inserted in one batch and one transaction (no GO, because variables do not
--     survive a GO). XACT_ABORT rolls the whole seed back if any statement fails.
--   * The data respects the domain rules, because aggregates are rebuilt from these rows through
--     Reconstitute(): occupied rooms have open stays, dirty rooms have a pending/in-progress
--     cleaning task, settled folios have a zero balance (charges - payments - adjustments),
--     allotment BookedCount equals the non-cancelled reservations covering each night.
-- ============================================================================================
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF EXISTS (SELECT 1
           FROM guestmgmt.Guest) OR EXISTS (SELECT 1
                                            FROM roominventory.RoomTypeDefinition)
    BEGIN
        PRINT 'Seed skipped: the database already contains data.';
        RETURN;
    END;

DECLARE @Now DATETIME2 = SYSUTCDATETIME();
DECLARE @Today DATE = CAST(@Now AS DATE);

DECLARE @In_S1 DATETIME2 = DATEADD(MINUTE, 840, CAST(DATEADD(DAY, -2, @Today) AS DATETIME2));
DECLARE @In_S2 DATETIME2 = DATEADD(MINUTE, 900, CAST(DATEADD(DAY, -1, @Today) AS DATETIME2));
DECLARE @In_S3 DATETIME2 = DATEADD(MINUTE, 810, CAST(DATEADD(DAY, -2, @Today) AS DATETIME2));
DECLARE @In_S4 DATETIME2 = DATEADD(MINUTE, 825, CAST(DATEADD(DAY, -2, @Today) AS DATETIME2));
DECLARE @In_S5 DATETIME2 = DATEADD(MINUTE, 855, CAST(DATEADD(DAY, -6, @Today) AS DATETIME2));
DECLARE @Out_S5 DATETIME2 = DATEADD(MINUTE, 630, CAST(DATEADD(DAY, -3, @Today) AS DATETIME2));
DECLARE @In_S6 DATETIME2 = DATEADD(MINUTE, 900, CAST(DATEADD(DAY, -9, @Today) AS DATETIME2));
DECLARE @Out_S6 DATETIME2 = DATEADD(MINUTE, 660, CAST(DATEADD(DAY, -7, @Today) AS DATETIME2));
DECLARE @In_S7 DATETIME2 = DATEADD(MINUTE, 960, CAST(DATEADD(DAY, -3, @Today) AS DATETIME2));
DECLARE @Out_S7 DATETIME2 = DATEADD(MINUTE, -120, @Now);
DECLARE @In_S8 DATETIME2 = DATEADD(MINUTE, 930, CAST(DATEADD(DAY, -4, @Today) AS DATETIME2));
DECLARE @Out_S8 DATETIME2 = DATEADD(MINUTE, 615, CAST(DATEADD(DAY, -1, @Today) AS DATETIME2));

BEGIN TRANSACTION;

-- 0001 guestmgmt.Guest ------------------------------------------------------------------
INSERT INTO guestmgmt.Guest (Id, FirstName, LastName, MiddleName, DateOfBirth, PhoneNumber, Email, IdentityDocumentType,
                             IdentityDocumentNumber, IsActive, CreatedAt)
VALUES ('11111111-0000-0000-0000-000000000001', N'Kwame', N'Mensah', NULL, '1985-03-14', N'+233244123456',
        N'kwame.mensah@example.com', N'GhanaCard', N'GHA-723456789-1', 1, DATEADD(DAY, -120, @Now)), -- Kwame Mensah
       ('11111111-0000-0000-0000-000000000002', N'Ama', N'Boateng', N'Serwaa', '1990-07-22', N'+233201234567',
        N'ama.boateng@example.com', N'Passport', N'G1234567', 1, DATEADD(DAY, -95, @Now)),           -- Ama Boateng
       ('11111111-0000-0000-0000-000000000003', N'Kofi', N'Asante', NULL, '1978-11-02', N'+233277654321', NULL,
        N'DriversLicense', N'GHDL-456123', 1, DATEADD(DAY, -80, @Now)),                              -- Kofi Asante
       ('11111111-0000-0000-0000-000000000004', N'Abena', N'Owusu', NULL, '1993-01-30', N'+233553214567',
        N'abena.owusu@example.com', N'GhanaCard', N'GHA-812345678-4', 1, DATEADD(DAY, -70, @Now)),   -- Abena Owusu
       ('11111111-0000-0000-0000-000000000005', N'Yaw', N'Darko', NULL, '1982-09-09', N'+233243001122', NULL,
        N'NationalId', N'NID-2019-004512', 1, DATEADD(DAY, -60, @Now)),                              -- Yaw Darko
       ('11111111-0000-0000-0000-000000000006', N'Efua', N'Quartey', NULL, '1988-05-17', N'+233208889900',
        N'efua.quartey@example.com', N'Passport', N'P9876543', 1, DATEADD(DAY, -55, @Now)),          -- Efua Quartey
       ('11111111-0000-0000-0000-000000000007', N'Nana', N'Adjei', NULL, '1975-12-25', N'+233244778899',
        N'nana.adjei@example.com', N'GhanaCard', N'GHA-701234567-8', 1, DATEADD(DAY, -50, @Now)),    -- Nana Adjei
       ('11111111-0000-0000-0000-000000000008', N'Esi', N'Appiah', NULL, '1996-04-04', N'+233269990011', NULL,
        N'GhanaCard', N'GHA-934567890-2', 1, DATEADD(DAY, -45, @Now)),                               -- Esi Appiah
       ('11111111-0000-0000-0000-000000000009', N'Michael', N'Thompson', NULL, '1980-06-18', N'+14155550123',
        N'michael.thompson@example.com', N'Passport', N'US5512340', 1, DATEADD(DAY, -10, @Now)),     -- Michael Thompson
       ('11111111-0000-0000-0000-000000000010', N'Sophie', N'Laurent', NULL, '1991-10-03', N'+33612345678',
        N'sophie.laurent@example.com', N'Passport', N'FR7788123', 1, DATEADD(DAY, -10, @Now)),       -- Sophie Laurent
       ('11111111-0000-0000-0000-000000000011', N'Kojo', N'Annan', NULL, '1969-02-11', N'+233244556677', NULL,
        N'DriversLicense', N'GHDL-998877', 0, DATEADD(DAY, -200, @Now)),                             -- Kojo Annan
       ('11111111-0000-0000-0000-000000000012', N'Akosua', N'Frimpong', NULL, '1987-08-28', N'+233266112233',
        N'akosua.frimpong@example.com', N'GhanaCard', N'GHA-645678901-3', 1, DATEADD(DAY, -30, @Now));
-- Akosua Frimpong

-- 0003 roominventory.RoomTypeDefinition -------------------------------------------------
INSERT INTO roominventory.RoomTypeDefinition (Id, Name, MaxOccupancy, BaseRateAmount, BaseRateCurrency)
VALUES ('22222222-0000-0000-0000-000000000001', N'Standard Single', 1, 350.00, 'GHS'),
       ('22222222-0000-0000-0000-000000000002', N'Standard Double', 2, 520.00, 'GHS'),
       ('22222222-0000-0000-0000-000000000003', N'Deluxe King', 2, 780.00, 'GHS'),
       ('22222222-0000-0000-0000-000000000004', N'Executive Suite', 4, 1450.00, 'GHS');

-- 0004 roominventory.Room ---------------------------------------------------------------
INSERT INTO roominventory.Room (Id, RoomNumber, FloorLevel, FloorWing, RoomTypeId, Status)
VALUES ('33333333-0000-0000-0000-000000000101', N'101', 1, N'East Wing', '22222222-0000-0000-0000-000000000001',
        N'Vacant'),
       ('33333333-0000-0000-0000-000000000102', N'102', 1, N'East Wing', '22222222-0000-0000-0000-000000000001',
        N'Occupied'),
       ('33333333-0000-0000-0000-000000000103', N'103', 1, NULL, '22222222-0000-0000-0000-000000000001', N'Vacant'),
       ('33333333-0000-0000-0000-000000000104', N'104', 1, N'West Wing', '22222222-0000-0000-0000-000000000002',
        N'Occupied'),
       ('33333333-0000-0000-0000-000000000105', N'105', 1, N'West Wing', '22222222-0000-0000-0000-000000000002',
        N'Vacant'),
       ('33333333-0000-0000-0000-000000000201', N'201', 2, N'East Wing', '22222222-0000-0000-0000-000000000002',
        N'Dirty'),
       ('33333333-0000-0000-0000-000000000202', N'202', 2, N'East Wing', '22222222-0000-0000-0000-000000000002',
        N'Vacant'),
       ('33333333-0000-0000-0000-000000000203', N'203', 2, N'West Wing', '22222222-0000-0000-0000-000000000003',
        N'Occupied'),
       ('33333333-0000-0000-0000-000000000204', N'204', 2, N'West Wing', '22222222-0000-0000-0000-000000000003',
        N'Dirty'),
       ('33333333-0000-0000-0000-000000000301', N'301', 3, N'East Wing', '22222222-0000-0000-0000-000000000003',
        N'Occupied'),
       ('33333333-0000-0000-0000-000000000302', N'302', 3, N'East Wing', '22222222-0000-0000-0000-000000000003',
        N'Vacant'),
       ('33333333-0000-0000-0000-000000000303', N'303', 3, N'West Wing', '22222222-0000-0000-0000-000000000004',
        N'Vacant'),
       ('33333333-0000-0000-0000-000000000304', N'304', 3, N'West Wing', '22222222-0000-0000-0000-000000000004',
        N'OutOfService');

-- 0005 roominventory.RoomTypeAllotment --------------------------------------------------
-- One row per room type per night, today-14 .. today+60. TotalRooms = physical rooms of that type;
-- BookedCount is filled in below, once the reservations exist.
;
WITH Dates AS
         (SELECT DATEADD(DAY, -14, @Today) AS [Date]
          UNION ALL
          SELECT DATEADD(DAY, 1, [Date])
          FROM Dates
          WHERE [Date] < DATEADD(DAY, 60, @Today))
INSERT
INTO roominventory.RoomTypeAllotment (Id, RoomTypeId, [Date], TotalRooms, BookedCount)
SELECT NEWID(), rt.Id, dt.[Date], rc.RoomCount, 0
FROM roominventory.RoomTypeDefinition rt
         CROSS JOIN Dates dt
         CROSS APPLY (SELECT COUNT(*) AS RoomCount FROM roominventory.Room ro WHERE ro.RoomTypeId = rt.Id) rc
OPTION (MAXRECURSION 200);

-- 0006 reservations.GroupBooking --------------------------------------------------------
INSERT INTO reservations.GroupBooking (Id, OrganizerGuestId, GroupName)
VALUES ('44444444-0000-0000-0000-000000000001', '11111111-0000-0000-0000-000000000012',
        N'Frimpong-Mensah Wedding Party'),
       ('44444444-0000-0000-0000-000000000002', '11111111-0000-0000-0000-000000000007',
        N'Ghana Tech Summit - Delegate Block');

-- 0007 reservations.Reservation ---------------------------------------------------------
INSERT INTO reservations.Reservation (Id, GuestId, GroupBookingId, RoomTypeRequestedId, StayStart, StayEnd,
                                      NightlyRateAmount, NightlyRateCurrency, FreeCancellationWindowHrs,
                                      CancellationFeeAmount, CancellationFeeCurrency, Status)
VALUES ('55555555-0000-0000-0000-000000000001', '11111111-0000-0000-0000-000000000001', NULL,
        '22222222-0000-0000-0000-000000000001', DATEADD(DAY, -2, @Today), DATEADD(DAY, 1, @Today), 350.00, 'GHS', 48,
        350.00, 'GHS', N'CheckedIn'),                                                -- Kwame Mensah, CheckedIn
       ('55555555-0000-0000-0000-000000000002', '11111111-0000-0000-0000-000000000009', NULL,
        '22222222-0000-0000-0000-000000000003', DATEADD(DAY, -1, @Today), DATEADD(DAY, 3, @Today), 780.00, 'GHS', 48,
        780.00, 'GHS', N'CheckedIn'),                                                -- Michael Thompson, CheckedIn
       ('55555555-0000-0000-0000-000000000003', '11111111-0000-0000-0000-000000000007',
        '44444444-0000-0000-0000-000000000002', '22222222-0000-0000-0000-000000000002', DATEADD(DAY, -2, @Today),
        DATEADD(DAY, 2, @Today), 468.00, 'GHS', 48, 468.00, 'GHS', N'CheckedIn'),    -- Nana Adjei, CheckedIn
       ('55555555-0000-0000-0000-000000000004', '11111111-0000-0000-0000-000000000003',
        '44444444-0000-0000-0000-000000000002', '22222222-0000-0000-0000-000000000003', DATEADD(DAY, -2, @Today),
        DATEADD(DAY, 2, @Today), 702.00, 'GHS', 48, 702.00, 'GHS', N'CheckedIn'),    -- Kofi Asante, CheckedIn
       ('55555555-0000-0000-0000-000000000005', '11111111-0000-0000-0000-000000000006', NULL,
        '22222222-0000-0000-0000-000000000002', DATEADD(DAY, -6, @Today), DATEADD(DAY, -3, @Today), 520.00, 'GHS', 48,
        520.00, 'GHS', N'CheckedOut'),                                               -- Efua Quartey, CheckedOut
       ('55555555-0000-0000-0000-000000000006', '11111111-0000-0000-0000-000000000005', NULL,
        '22222222-0000-0000-0000-000000000001', DATEADD(DAY, -9, @Today), DATEADD(DAY, -7, @Today), 350.00, 'GHS', 48,
        350.00, 'GHS', N'CheckedOut'),                                               -- Yaw Darko, CheckedOut
       ('55555555-0000-0000-0000-000000000007', '11111111-0000-0000-0000-000000000004', NULL,
        '22222222-0000-0000-0000-000000000002', DATEADD(DAY, 0, @Today), DATEADD(DAY, 2, @Today), 520.00, 'GHS', 48,
        520.00, 'GHS', N'Confirmed'),                                                -- Abena Owusu, Confirmed
       ('55555555-0000-0000-0000-000000000008', '11111111-0000-0000-0000-000000000008', NULL,
        '22222222-0000-0000-0000-000000000003', DATEADD(DAY, 1, @Today), DATEADD(DAY, 4, @Today), 780.00, 'GHS', 48,
        780.00, 'GHS', N'Confirmed'),                                                -- Esi Appiah, Confirmed
       ('55555555-0000-0000-0000-000000000009', '11111111-0000-0000-0000-000000000006', NULL,
        '22222222-0000-0000-0000-000000000004', DATEADD(DAY, 20, @Today), DATEADD(DAY, 23, @Today), 1450.00, 'GHS', 72,
        1450.00, 'GHS', N'Tentative'),                                               -- Efua Quartey, Tentative
       ('55555555-0000-0000-0000-000000000010', '11111111-0000-0000-0000-000000000001', NULL,
        '22222222-0000-0000-0000-000000000002', DATEADD(DAY, 1, @Today), DATEADD(DAY, 3, @Today), 520.00, 'GHS', 48,
        520.00, 'GHS', N'Cancelled'),                                                -- Kwame Mensah, Cancelled
       ('55555555-0000-0000-0000-000000000011', '11111111-0000-0000-0000-000000000008', NULL,
        '22222222-0000-0000-0000-000000000001', DATEADD(DAY, -4, @Today), DATEADD(DAY, -2, @Today), 350.00, 'GHS', 48,
        350.00, 'GHS', N'NoShow'),                                                   -- Esi Appiah, NoShow
       ('55555555-0000-0000-0000-000000000012', '11111111-0000-0000-0000-000000000012',
        '44444444-0000-0000-0000-000000000001', '22222222-0000-0000-0000-000000000002', DATEADD(DAY, 10, @Today),
        DATEADD(DAY, 13, @Today), 468.00, 'GHS', 48, 468.00, 'GHS', N'Confirmed'),   -- Akosua Frimpong, Confirmed
       ('55555555-0000-0000-0000-000000000013', '11111111-0000-0000-0000-000000000004',
        '44444444-0000-0000-0000-000000000001', '22222222-0000-0000-0000-000000000002', DATEADD(DAY, 10, @Today),
        DATEADD(DAY, 13, @Today), 468.00, 'GHS', 48, 468.00, 'GHS', N'Confirmed'),   -- Abena Owusu, Confirmed
       ('55555555-0000-0000-0000-000000000014', '11111111-0000-0000-0000-000000000008',
        '44444444-0000-0000-0000-000000000001', '22222222-0000-0000-0000-000000000004', DATEADD(DAY, 10, @Today),
        DATEADD(DAY, 13, @Today), 1305.00, 'GHS', 48, 1305.00, 'GHS', N'Confirmed'), -- Esi Appiah, Confirmed
       ('55555555-0000-0000-0000-000000000015', '11111111-0000-0000-0000-000000000010', NULL,
        '22222222-0000-0000-0000-000000000001', DATEADD(DAY, 14, @Today), DATEADD(DAY, 16, @Today), 350.00, 'GHS', 48,
        350.00, 'GHS', N'Confirmed'),                                                -- Sophie Laurent, Confirmed
       ('55555555-0000-0000-0000-000000000016', '11111111-0000-0000-0000-000000000002', NULL,
        '22222222-0000-0000-0000-000000000002', DATEADD(DAY, -3, @Today), DATEADD(DAY, 0, @Today), 520.00, 'GHS', 48,
        520.00, 'GHS', N'CheckedOut'),                                               -- Ama Boateng, CheckedOut
       ('55555555-0000-0000-0000-000000000017', '11111111-0000-0000-0000-000000000011', NULL,
        '22222222-0000-0000-0000-000000000003', DATEADD(DAY, -4, @Today), DATEADD(DAY, -1, @Today), 780.00, 'GHS', 48,
        780.00, 'GHS', N'CheckedOut');
-- Kojo Annan, CheckedOut

-- Reservation.Reserve() takes one unit of capacity per night; Cancel() releases it, NoShow does not.
UPDATE a
SET BookedCount = (SELECT COUNT(*)
                   FROM reservations.Reservation r
                   WHERE r.RoomTypeRequestedId = a.RoomTypeId
                     AND r.Status <> N'Cancelled'
                     AND a.[Date] >= r.StayStart
                     AND a.[Date] < r.StayEnd)
FROM roominventory.RoomTypeAllotment a;

-- 0008 frontdesk.Stay -------------------------------------------------------------------
INSERT INTO frontdesk.Stay (Id, ReservationId, GroupBookingId, AssignedRoomId, AssignedRoomNumber, MaxOccupancy,
                            ActualCheckIn, ActualCheckOut)
VALUES ('66666666-0000-0000-0000-000000000001', '55555555-0000-0000-0000-000000000001', NULL,
        '33333333-0000-0000-0000-000000000102', N'102', 1, @In_S1, NULL),
       ('66666666-0000-0000-0000-000000000002', '55555555-0000-0000-0000-000000000002', NULL,
        '33333333-0000-0000-0000-000000000203', N'203', 2, @In_S2, NULL),
       ('66666666-0000-0000-0000-000000000003', '55555555-0000-0000-0000-000000000003',
        '44444444-0000-0000-0000-000000000002', '33333333-0000-0000-0000-000000000104', N'104', 2, @In_S3, NULL),
       ('66666666-0000-0000-0000-000000000004', '55555555-0000-0000-0000-000000000004',
        '44444444-0000-0000-0000-000000000002', '33333333-0000-0000-0000-000000000301', N'301', 2, @In_S4, NULL),
       ('66666666-0000-0000-0000-000000000005', '55555555-0000-0000-0000-000000000005', NULL,
        '33333333-0000-0000-0000-000000000105', N'105', 2, @In_S5, @Out_S5),
       ('66666666-0000-0000-0000-000000000006', '55555555-0000-0000-0000-000000000006', NULL,
        '33333333-0000-0000-0000-000000000101', N'101', 1, @In_S6, @Out_S6),
       ('66666666-0000-0000-0000-000000000007', '55555555-0000-0000-0000-000000000016', NULL,
        '33333333-0000-0000-0000-000000000201', N'201', 2, @In_S7, @Out_S7),
       ('66666666-0000-0000-0000-000000000008', '55555555-0000-0000-0000-000000000017', NULL,
        '33333333-0000-0000-0000-000000000204', N'204', 2, @In_S8, @Out_S8);

-- frontdesk.StayOccupant
INSERT INTO frontdesk.StayOccupant (StayId, GuestId)
VALUES ('66666666-0000-0000-0000-000000000001', '11111111-0000-0000-0000-000000000001'), -- Kwame Mensah
       ('66666666-0000-0000-0000-000000000002', '11111111-0000-0000-0000-000000000009'), -- Michael Thompson
       ('66666666-0000-0000-0000-000000000002', '11111111-0000-0000-0000-000000000010'), -- Sophie Laurent
       ('66666666-0000-0000-0000-000000000003', '11111111-0000-0000-0000-000000000007'), -- Nana Adjei
       ('66666666-0000-0000-0000-000000000004', '11111111-0000-0000-0000-000000000003'), -- Kofi Asante
       ('66666666-0000-0000-0000-000000000005', '11111111-0000-0000-0000-000000000006'), -- Efua Quartey
       ('66666666-0000-0000-0000-000000000006', '11111111-0000-0000-0000-000000000005'), -- Yaw Darko
       ('66666666-0000-0000-0000-000000000007', '11111111-0000-0000-0000-000000000002'), -- Ama Boateng
       ('66666666-0000-0000-0000-000000000008', '11111111-0000-0000-0000-000000000011');
-- Kojo Annan

-- 0009 housekeeping.HouseKeepingTask ----------------------------------------------------
-- AssignedStaffId values are placeholders (no FK, no auth.[User] rows are seeded).
INSERT INTO housekeeping.HouseKeepingTask (Id, RoomId, Type, Status, AssignedStaffId, CreatedAt, CompletedAt,
                                           InspectionPassed, InspectionNotes)
VALUES ('77777777-0000-0000-0000-000000000001', '33333333-0000-0000-0000-000000000201', N'Cleaning', N'Pending', NULL,
        @Out_S7, NULL, NULL, NULL),                                                              -- room 201
       ('77777777-0000-0000-0000-000000000002', '33333333-0000-0000-0000-000000000204', N'Cleaning', N'InProgress',
        '99999999-0000-0000-0000-000000000001', @Out_S8, NULL, NULL, NULL),                      -- room 204
       ('77777777-0000-0000-0000-000000000003', '33333333-0000-0000-0000-000000000304', N'Maintenance', N'Pending',
        NULL, DATEADD(MINUTE, -1440, @Now), NULL, NULL, NULL),                                   -- room 304
       ('77777777-0000-0000-0000-000000000004', '33333333-0000-0000-0000-000000000105', N'Cleaning', N'Completed',
        '99999999-0000-0000-0000-000000000001', @Out_S5, DATEADD(MINUTE, 90, @Out_S5), 1, NULL), -- room 105
       ('77777777-0000-0000-0000-000000000005', '33333333-0000-0000-0000-000000000105', N'Inspection', N'Failed',
        '99999999-0000-0000-0000-000000000002', DATEADD(MINUTE, 120, @Out_S5), DATEADD(MINUTE, 150, @Out_S5), 0,
        N'Bathroom grout needs re-cleaning, minibar not restocked.'),                            -- room 105
       ('77777777-0000-0000-0000-000000000006', '33333333-0000-0000-0000-000000000105', N'Cleaning', N'Completed',
        '99999999-0000-0000-0000-000000000001', DATEADD(MINUTE, 180, @Out_S5), DATEADD(MINUTE, 240, @Out_S5), 1,
        NULL),                                                                                   -- room 105
       ('77777777-0000-0000-0000-000000000007', '33333333-0000-0000-0000-000000000105', N'Inspection', N'Completed',
        '99999999-0000-0000-0000-000000000002', DATEADD(MINUTE, 270, @Out_S5), DATEADD(MINUTE, 300, @Out_S5), 1,
        N'Re-inspection passed.'),                                                               -- room 105
       ('77777777-0000-0000-0000-000000000008', '33333333-0000-0000-0000-000000000101', N'Cleaning', N'Completed',
        '99999999-0000-0000-0000-000000000002', @Out_S6, DATEADD(MINUTE, 60, @Out_S6), 1, NULL), -- room 101
       ('77777777-0000-0000-0000-000000000009', '33333333-0000-0000-0000-000000000302', N'Inspection', N'Completed',
        '99999999-0000-0000-0000-000000000001', DATEADD(MINUTE, -7200, @Now), DATEADD(MINUTE, -7170, @Now), 1,
        N'Routine inspection - no issues.');
-- room 302

-- 0010 billing.Folio --------------------------------------------------------------------
INSERT INTO billing.Folio (Id, OwnerType, OwnerId, Currency, Status)
VALUES ('88888888-0000-0000-0000-000000000001', N'Stay', '66666666-0000-0000-0000-000000000001', 'GHS', N'Open'),
       ('88888888-0000-0000-0000-000000000002', N'Stay', '66666666-0000-0000-0000-000000000002', 'GHS', N'Open'),
       ('88888888-0000-0000-0000-000000000003', N'GuestWithinStay', '11111111-0000-0000-0000-000000000010', 'GHS',
        N'Open'),
       ('88888888-0000-0000-0000-000000000004', N'Stay', '66666666-0000-0000-0000-000000000003', 'GHS', N'Open'),
       ('88888888-0000-0000-0000-000000000005', N'Stay', '66666666-0000-0000-0000-000000000004', 'GHS', N'Open'),
       ('88888888-0000-0000-0000-000000000006', N'GroupBooking', '44444444-0000-0000-0000-000000000002', 'GHS',
        N'Open'),
       ('88888888-0000-0000-0000-000000000007', N'GroupBooking', '44444444-0000-0000-0000-000000000001', 'GHS',
        N'Open'),
       ('88888888-0000-0000-0000-000000000008', N'Stay', '66666666-0000-0000-0000-000000000005', 'GHS', N'Settled'),
       ('88888888-0000-0000-0000-000000000009', N'Stay', '66666666-0000-0000-0000-000000000006', 'GHS', N'Settled'),
       ('88888888-0000-0000-0000-000000000010', N'Stay', '66666666-0000-0000-0000-000000000007', 'GHS', N'Settled'),
       ('88888888-0000-0000-0000-000000000011', N'Stay', '66666666-0000-0000-0000-000000000008', 'GHS', N'Settled'),
       ('88888888-0000-0000-0000-000000000012', N'Guest', '11111111-0000-0000-0000-000000000001', 'GHS', N'Open'),
       ('88888888-0000-0000-0000-000000000013', N'Guest', '11111111-0000-0000-0000-000000000008', 'GHS', N'Settled'),
       ('88888888-0000-0000-0000-000000000014', N'Guest', '11111111-0000-0000-0000-000000000004', 'GHS', N'Void');

-- billing.Charge (Id is an IDENTITY column)
INSERT INTO billing.Charge (FolioId, Amount, Currency, Category, Description, PostedAt)
VALUES ('88888888-0000-0000-0000-000000000001', 1050.00, 'GHS', N'Room Charge', N'3 nights x GHS 350.00', @In_S1),
       ('88888888-0000-0000-0000-000000000001', 85.50, 'GHS', N'Restaurant', N'Dinner - Akoma Restaurant',
        DATEADD(MINUTE, 360, @In_S1)),
       ('88888888-0000-0000-0000-000000000001', 24.00, 'GHS', N'Minibar', N'Minibar - 2 soft drinks',
        DATEADD(MINUTE, 1440, @In_S1)),
       ('88888888-0000-0000-0000-000000000002', 3120.00, 'GHS', N'Room Charge', N'4 nights x GHS 780.00', @In_S2),
       ('88888888-0000-0000-0000-000000000002', 210.00, 'GHS', N'Restaurant', N'Dinner - Akoma Restaurant',
        DATEADD(MINUTE, 240, @In_S2)),
       ('88888888-0000-0000-0000-000000000002', 45.00, 'GHS', N'Laundry', N'Laundry - 5 items',
        DATEADD(MINUTE, 270, @In_S2)),
       ('88888888-0000-0000-0000-000000000003', 120.00, 'GHS', N'Restaurant', N'Room service - dinner',
        DATEADD(MINUTE, 300, @In_S2)),
       ('88888888-0000-0000-0000-000000000004', 1872.00, 'GHS', N'Room Charge', N'4 nights x GHS 468.00 (group rate)',
        @In_S3),
       ('88888888-0000-0000-0000-000000000005', 2808.00, 'GHS', N'Room Charge', N'4 nights x GHS 702.00 (group rate)',
        @In_S4),
       ('88888888-0000-0000-0000-000000000006', 3000.00, 'GHS', N'Conference Package',
        N'Delegate block - conference package', DATEADD(MINUTE, 600, CAST(DATEADD(DAY, -3, @Today) AS DATETIME2))),
       ('88888888-0000-0000-0000-000000000007', 2500.00, 'GHS', N'Event Hall Hire', N'Wedding reception hall hire',
        DATEADD(MINUTE, 600, CAST(DATEADD(DAY, -20, @Today) AS DATETIME2))),
       ('88888888-0000-0000-0000-000000000008', 1560.00, 'GHS', N'Room Charge', N'3 nights x GHS 520.00', @In_S5),
       ('88888888-0000-0000-0000-000000000008', 120.00, 'GHS', N'Restaurant', N'Dinner - Akoma Restaurant',
        DATEADD(MINUTE, 1440, @In_S5)),
       ('88888888-0000-0000-0000-000000000009', 700.00, 'GHS', N'Room Charge', N'2 nights x GHS 350.00', @In_S6),
       ('88888888-0000-0000-0000-000000000009', 40.00, 'GHS', N'Minibar', N'Minibar - snacks',
        DATEADD(MINUTE, 1440, @In_S6)),
       ('88888888-0000-0000-0000-000000000010', 1560.00, 'GHS', N'Room Charge', N'3 nights x GHS 520.00', @In_S7),
       ('88888888-0000-0000-0000-000000000010', 95.00, 'GHS', N'Restaurant', N'Dinner - Akoma Restaurant',
        DATEADD(MINUTE, 1440, @In_S7)),
       ('88888888-0000-0000-0000-000000000010', 30.00, 'GHS', N'Laundry', N'Laundry - 3 items',
        DATEADD(MINUTE, 2000, @In_S7)),
       ('88888888-0000-0000-0000-000000000011', 2340.00, 'GHS', N'Room Charge', N'3 nights x GHS 780.00', @In_S8),
       ('88888888-0000-0000-0000-000000000011', 55.50, 'GHS', N'Minibar', N'Minibar - drinks',
        DATEADD(MINUTE, 1440, @In_S8)),
       ('88888888-0000-0000-0000-000000000011', 55.50, 'GHS', N'Minibar', N'Minibar - drinks',
        DATEADD(MINUTE, 1445, @In_S8)),
       ('88888888-0000-0000-0000-000000000012', 520.00, 'GHS', N'Cancellation Fee',
        N'Cancellation fee for reservation 55555555-0000-0000-0000-000000000010', DATEADD(MINUTE, -180, @Now)),
       ('88888888-0000-0000-0000-000000000013', 350.00, 'GHS', N'No-Show Fee',
        N'No-show fee for reservation 55555555-0000-0000-0000-000000000011',
        DATEADD(MINUTE, 600, CAST(DATEADD(DAY, -3, @Today) AS DATETIME2)));

-- billing.Payment
INSERT INTO billing.Payment (FolioId, Amount, Currency, Method, Reference, ReceivedAt)
VALUES ('88888888-0000-0000-0000-000000000001', 350.00, 'GHS', N'Cash', NULL, @In_S1),
       ('88888888-0000-0000-0000-000000000002', 1500.00, 'GHS', N'Card', N'POS-481516', DATEADD(MINUTE, 5, @In_S2)),
       ('88888888-0000-0000-0000-000000000003', 50.00, 'GHS', N'Cash', NULL, DATEADD(MINUTE, 310, @In_S2)),
       ('88888888-0000-0000-0000-000000000005', 1000.00, 'GHS', N'MobileMoney', N'MM-77451',
        DATEADD(MINUTE, 10, @In_S4)),
       ('88888888-0000-0000-0000-000000000006', 2000.00, 'GHS', N'BankTransfer', N'TRF-20931',
        DATEADD(MINUTE, 660, CAST(DATEADD(DAY, -3, @Today) AS DATETIME2))),
       ('88888888-0000-0000-0000-000000000007', 1000.00, 'GHS', N'BankTransfer', N'TRF-WED-0042',
        DATEADD(MINUTE, 660, CAST(DATEADD(DAY, -20, @Today) AS DATETIME2))),
       ('88888888-0000-0000-0000-000000000008', 1000.00, 'GHS', N'Card', N'POS-390211', @Out_S5),
       ('88888888-0000-0000-0000-000000000008', 680.00, 'GHS', N'Cash', NULL, DATEADD(MINUTE, 5, @Out_S5)),
       ('88888888-0000-0000-0000-000000000009', 700.00, 'GHS', N'Cash', NULL, @Out_S6),
       ('88888888-0000-0000-0000-000000000010', 1685.00, 'GHS', N'Card', N'POS-552871', @Out_S7),
       ('88888888-0000-0000-0000-000000000011', 2000.00, 'GHS', N'Cash', NULL, @Out_S8),
       ('88888888-0000-0000-0000-000000000011', 395.50, 'GHS', N'MobileMoney', N'MM-88213',
        DATEADD(MINUTE, 5, @Out_S8)),
       ('88888888-0000-0000-0000-000000000013', 350.00, 'GHS', N'MobileMoney', N'MM-61902',
        DATEADD(MINUTE, 600, CAST(DATEADD(DAY, -2, @Today) AS DATETIME2)));

-- billing.Adjustment
INSERT INTO billing.Adjustment (FolioId, Amount, Currency, Type, Reason, PostedAt)
VALUES ('88888888-0000-0000-0000-000000000009', 40.00, 'GHS', N'GoodwillCredit',
        N'Minibar charge waived after delayed check-in', DATEADD(MINUTE, 5, @Out_S6)),
       ('88888888-0000-0000-0000-000000000011', 55.50, 'GHS', N'BillingCorrection',
        N'Duplicate minibar posting removed', DATEADD(MINUTE, 10, @Out_S8));

-- Self-checks: if someone edits this file and breaks a rule, fail loudly and roll everything back.
IF EXISTS (SELECT 1
           FROM billing.Folio f
           WHERE f.Status = N'Settled'
             AND (SELECT COALESCE(SUM(Amount), 0) FROM billing.Charge WHERE FolioId = f.Id)
                     - (SELECT COALESCE(SUM(Amount), 0) FROM billing.Payment WHERE FolioId = f.Id)
                     - (SELECT COALESCE(SUM(Amount), 0) FROM billing.Adjustment WHERE FolioId = f.Id) <> 0)
    THROW 50001, 'Seed error: a Settled folio has a non-zero balance.', 1;

IF (SELECT COUNT(*)
    FROM roominventory.Room
    WHERE Status = N'Occupied')
    <> (SELECT COUNT(*)
        FROM frontdesk.Stay
        WHERE ActualCheckOut IS NULL)
    THROW 50002, 'Seed error: Occupied rooms and open stays do not match.', 1;

IF EXISTS (SELECT 1
           FROM frontdesk.Stay s
           WHERE (SELECT COUNT(*) FROM frontdesk.StayOccupant o WHERE o.StayId = s.Id) > s.MaxOccupancy)
    THROW 50003, 'Seed error: a stay has more occupants than its snapshotted MaxOccupancy.', 1;

COMMIT TRANSACTION;

PRINT 'Demo data seeded.';