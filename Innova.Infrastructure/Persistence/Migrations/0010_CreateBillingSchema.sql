-- 0010_CreateBillingSchema.sql
CREATE SCHEMA billing;
GO

CREATE TABLE billing.Folio
(
    Id        UNIQUEIDENTIFIER PRIMARY KEY,
    OwnerType NVARCHAR(20)     NOT NULL,
    -- No FK: OwnerId is polymorphic across four possible owner tables
    -- depending on OwnerType (Stay/GuestWithinStay/GroupBooking/Guest).
    -- SQL Server has no native conditional foreign key, so this is
    -- enforced at the application layer only — matches the domain-level
    -- decoupling decision behind FolioOwner itself.
    OwnerId   UNIQUEIDENTIFIER NOT NULL,
    Currency  CHAR(3)          NOT NULL,
    Status    NVARCHAR(20)     NOT NULL,
    CONSTRAINT CK_Folio_OwnerType
        CHECK (OwnerType IN ('Stay', 'GuestWithinStay', 'GroupBooking', 'Guest')),
    CONSTRAINT CK_Folio_Status
        CHECK (Status IN ('Open', 'Settled', 'Void'))
);
GO

CREATE INDEX IX_Folio_OwnerType_OwnerId
    ON billing.Folio (OwnerType, OwnerId);
GO

-- Charge, Payment, and Adjustment are pure value objects with no domain-
-- level identity — each gets a synthetic Id here purely so the row can be
-- targeted for persistence; the domain never sees or uses it.
CREATE TABLE billing.Charge
(
    Id          INT IDENTITY PRIMARY KEY,
    FolioId     UNIQUEIDENTIFIER NOT NULL,
    Amount      DECIMAL(18, 2)   NOT NULL,
    Currency    CHAR(3)          NOT NULL,
    Category    NVARCHAR(100)    NOT NULL,
    Description NVARCHAR(500)    NULL,
    PostedAt    DATETIME2        NOT NULL,
    CONSTRAINT FK_Charge_Folio FOREIGN KEY (FolioId)
        REFERENCES billing.Folio (Id),
    -- Charge.Of() rejects < 0, not <= 0 — a zero charge is valid.
    CONSTRAINT CK_Charge_Amount CHECK (Amount >= 0)
);
GO

CREATE TABLE billing.Payment
(
    Id         INT IDENTITY PRIMARY KEY,
    FolioId    UNIQUEIDENTIFIER NOT NULL,
    Amount     DECIMAL(18, 2)   NOT NULL,
    Currency   CHAR(3)          NOT NULL,
    Method     NVARCHAR(20)     NOT NULL,
    Reference  NVARCHAR(200)    NULL,
    ReceivedAt DATETIME2        NOT NULL,
    CONSTRAINT FK_Payment_Folio FOREIGN KEY (FolioId)
        REFERENCES billing.Folio (Id),
    CONSTRAINT CK_Payment_Method
        CHECK (Method IN ('Cash', 'Card', 'BankTransfer', 'MobileMoney')),
    -- Payment.Of() rejects <= 0 — unlike Charge, zero isn't valid here.
    CONSTRAINT CK_Payment_Amount CHECK (Amount > 0)
);
GO

CREATE TABLE billing.Adjustment
(
    Id       INT IDENTITY PRIMARY KEY,
    FolioId  UNIQUEIDENTIFIER NOT NULL,
    Amount   DECIMAL(18, 2)   NOT NULL,
    Currency CHAR(3)          NOT NULL,
    Type     NVARCHAR(30)     NOT NULL,
    Reason   NVARCHAR(500)    NOT NULL,
    PostedAt DATETIME2        NOT NULL,
    CONSTRAINT FK_Adjustment_Folio FOREIGN KEY (FolioId)
        REFERENCES billing.Folio (Id),
    CONSTRAINT CK_Adjustment_Type
        CHECK (Type IN ('Refund', 'GoodwillCredit', 'BillingCorrection')),
    CONSTRAINT CK_Adjustment_Amount CHECK (Amount > 0)
);
GO

CREATE INDEX IX_Charge_FolioId ON billing.Charge (FolioId);
GO
CREATE INDEX IX_Payment_FolioId ON billing.Payment (FolioId);
GO
CREATE INDEX IX_Adjustment_FolioId ON billing.Adjustment (FolioId);
GO