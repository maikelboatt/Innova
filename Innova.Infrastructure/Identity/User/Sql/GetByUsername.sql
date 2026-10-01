-- GetByUsername.sql
SELECT Id, Username, PasswordHash, Role, IsActive
FROM auth.[User]
WHERE Username = @Username;