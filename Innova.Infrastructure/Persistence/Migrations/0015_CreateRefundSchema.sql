-- 0015_CreateRefundSchema.sql
-- Refunds become their own ledger line (money going back to the guest) instead of an adjustment type.
--
--   bill side:  Charge (+)   Adjustment (-)
--   cash side:  Payment (-)  Refund (+)        Balance = Charges - Adjustments - Payments + Refunds
--
-- Also adds billing.FolioBalance, a view holding the balance formula so every read query shares one copy of it.

-- Any Refund ADJUSTMENT written before this migration was subtracted from the balance (the wrong direction).
-- Do not guess what they meant: stop, and let a person decide how each one should be restated.
IF EXISTS (SELECT 1
           FROM billing.Adjustment
           WHERE Type = N'Refund')
    THROW 50010, 'billing.Adjustment contains Refund rows that were counted with the wrong sign. Review them by hand (restate each as a billing.Refund with the correct method) and delete them, then run this migration again.', 1;
GO

CREATE TABLE billing.Refund
(
    Id         INT IDENTITY PRIMARY KEY,
    FolioId    UNIQUEIDENTIFIER NOT NULL,
    Amount     DECIMAL(18, 2)   NOT NULL,
    Currency   CHAR(3)          NOT NULL,
    Method     NVARCHAR(20)     NOT NULL,
    Reference  NVARCHAR(200)    NULL,
    Reason     NVARCHAR(500)    NOT NULL,
    RefundedAt DATETIME2        NOT NULL,
    CONSTRAINT FK_Refund_Folio FOREIGN KEY (FolioId)
        REFERENCES billing.Folio (Id),
    CONSTRAINT CK_Refund_Method
        CHECK (Method IN ('Cash', 'Card', 'BankTransfer', 'MobileMoney')),
    -- Refund.Of() rejects <= 0, same as Payment.
    CONSTRAINT CK_Refund_Amount CHECK (Amount > 0)
);
GO

CREATE INDEX IX_Refund_FolioId ON billing.Refund (FolioId);
GO

-- Adjustments are credits only now.
ALTER TABLE billing.Adjustment
    DROP CONSTRAINT CK_Adjustment_Type;
GO
ALTER TABLE billing.Adjustment
    ADD CONSTRAINT CK_Adjustment_Type CHECK (Type IN ('GoodwillCredit', 'BillingCorrection'));
GO

-- The one SQL copy of the balance formula. Folio.Balance in the domain is the other copy; a test keeps them equal.
--   TotalPayments - TotalRefunds  = NetPayments   (cash actually held for the guest)
--   Balance < 0                    = the folio is in credit (the hotel holds the guest's money)
CREATE VIEW billing.FolioBalance
AS
SELECT f.Id                                                                   AS FolioId,
       t.TotalCharges,
       t.TotalPayments,
       t.TotalRefunds,
       t.TotalAdjustments,
       t.TotalPayments - t.TotalRefunds                                       AS NetPayments,
       t.TotalCharges - t.TotalAdjustments - t.TotalPayments + t.TotalRefunds AS Balance
FROM billing.Folio f
         CROSS APPLY (SELECT (SELECT COALESCE(SUM(c.Amount), 0)
                              FROM billing.Charge c
                              WHERE c.FolioId = f.Id)                                                             AS TotalCharges,
                             (SELECT COALESCE(SUM(p.Amount), 0)
                              FROM billing.Payment p
                              WHERE p.FolioId = f.Id)                                                             AS TotalPayments,
                             (SELECT COALESCE(SUM(r.Amount), 0)
                              FROM billing.Refund r
                              WHERE r.FolioId = f.Id)                                                             AS TotalRefunds,
                             (SELECT COALESCE(SUM(a.Amount), 0)
                              FROM billing.Adjustment a
                              WHERE a.FolioId = f.Id)                                                             AS TotalAdjustments) t;
GO