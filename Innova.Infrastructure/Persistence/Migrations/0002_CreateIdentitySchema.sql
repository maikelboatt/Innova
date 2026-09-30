-- 0002_CreateIdentitySchema.sql
CREATE SCHEMA identity;
GO

-- [User] is bracketed — USER is a reserved word in T-SQL.
CREATE TABLE identity.[User]
(
    Id           UNIQUEIDENTIFIER PRIMARY KEY,
    Username     NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,
    Role         NVARCHAR(50)  NOT NULL,
    IsActive     BIT           NOT NULL DEFAULT 1,
    CONSTRAINT UQ_User_Username UNIQUE (Username),
);
GO

CREATE UNIQUE INDEX UX_User_Username
    ON identity.[User] (Username);
GO