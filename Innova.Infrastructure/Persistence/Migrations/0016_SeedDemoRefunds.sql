-- 0016_SeedDemoRefunds.sql
-- ============================================================================================
-- DEMO / DEVELOPMENT DATA ONLY, and only meaningful on top of 0013_SeedDemoData.sql.
--
-- Adds three guest-owned folios that show refunds end to end:
--   folio 15  deposit paid, then returned in full                      -> Settled, balance 0
--   folio 16  deposit paid, a fee kept, the unused rest returned       -> Settled, balance 0
--   folio 17  200 paid with nothing owed, refund not issued yet        -> Open, in credit (Refund button enabled)
--
-- Runs only if the 0013 demo data is present and this seed has not been applied. One batch, one transaction.
-- Run it after 0014_CreateRefundSchema.sql (it needs billing.Refund and billing.FolioBalance).
-- ============================================================================================
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF NOT EXISTS (SELECT 1
               FROM billing.Folio
               WHERE Id = '88888888-0000-0000-0000-000000000001')
    OR EXISTS (SELECT 1
               FROM billing.Folio
               WHERE Id = '88888888-0000-0000-0000-000000000015')
    BEGIN
        PRINT 'Refund demo data skipped: the 0013 demo data is missing, or this seed was already applied.';
        RETURN;
    END;

DECLARE @Now DATETIME2 = SYSUTCDATETIME();
DECLARE @Today DATE = CAST(@Now AS DATE);

BEGIN TRANSACTION;

INSERT INTO billing.Folio (Id, OwnerType, OwnerId, Currency, Status)
VALUES ('88888888-0000-0000-0000-000000000015', N'Guest', '11111111-0000-0000-0000-000000000009', 'GHS',
        N'Settled'), -- Michael Thompson: pre-arrival deposit returned in full after the booking was amended
       ('88888888-0000-0000-0000-000000000016', N'Guest', '11111111-0000-0000-0000-000000000012', 'GHS',
        N'Settled'), -- Akosua Frimpong: wedding deposit, fee kept, the unused balance returned
       ('88888888-0000-0000-0000-000000000017', N'Guest', '11111111-0000-0000-0000-000000000008', 'GHS',
        N'Open'); -- Esi Appiah: paid 200 too much, refund still to be issued (shows the Refund button)

INSERT INTO billing.Charge (FolioId, Amount, Currency, Category, Description, PostedAt)
VALUES ('88888888-0000-0000-0000-000000000016', 300.00, 'GHS', N'Cancellation Fee', N'Fee for the reduced room block',
        DATEADD(MINUTE, 600, CAST(DATEADD(DAY, -8, @Today) AS DATETIME2)));

INSERT INTO billing.Payment (FolioId, Amount, Currency, Method, Reference, ReceivedAt)
VALUES ('88888888-0000-0000-0000-000000000015', 500.00, 'GHS', N'Card', N'POS-DEP-100',
        DATEADD(MINUTE, 600, CAST(DATEADD(DAY, -12, @Today) AS DATETIME2))),
       ('88888888-0000-0000-0000-000000000016', 1000.00, 'GHS', N'BankTransfer', N'TRF-DEP-WED-01',
        DATEADD(MINUTE, 600, CAST(DATEADD(DAY, -15, @Today) AS DATETIME2))),
       ('88888888-0000-0000-0000-000000000017', 200.00, 'GHS', N'Cash', NULL,
        DATEADD(MINUTE, 600, CAST(DATEADD(DAY, -1, @Today) AS DATETIME2)));

INSERT INTO billing.Refund (FolioId, Amount, Currency, Method, Reference, Reason, RefundedAt)
VALUES ('88888888-0000-0000-0000-000000000015', 500.00, 'GHS', N'Card', N'POS-REF-100',
        N'Deposit returned: booking amended', DATEADD(MINUTE, 660, CAST(DATEADD(DAY, -10, @Today) AS DATETIME2))),
       ('88888888-0000-0000-0000-000000000016', 700.00, 'GHS', N'BankTransfer', N'TRF-REF-WED-01',
        N'Unused deposit returned after the room block was reduced',
        DATEADD(MINUTE, 600, CAST(DATEADD(DAY, -7, @Today) AS DATETIME2)));

-- Self-checks (the same rules the domain enforces): fail loudly and roll everything back if they break.
IF EXISTS (SELECT 1
           FROM billing.Folio f
                    INNER JOIN billing.FolioBalance b ON b.FolioId = f.Id
           WHERE f.Status = N'Settled'
             AND b.Balance <> 0)
    THROW 50021, 'Seed error: a Settled folio has a non-zero balance.', 1;

IF EXISTS (SELECT 1
           FROM billing.FolioBalance
           WHERE TotalRefunds > TotalPayments)
    THROW 50022, 'Seed error: a folio has refunded more money than it ever received.', 1;

COMMIT TRANSACTION;

PRINT 'Demo refund data seeded.';