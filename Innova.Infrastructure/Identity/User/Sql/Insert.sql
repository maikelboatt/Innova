-- Insert.sql
INSERT INTO auth.[User] (Id, Username, PasswordHash, Role, IsActive)
VALUES (@Id, @Username, @PasswordHash, @Role, @IsActive);