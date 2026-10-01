-- Update.sql
UPDATE auth.[User]
SET PasswordHash = @PasswordHash, Role = @Role, IsActive = @IsActive
WHERE Id = @Id;