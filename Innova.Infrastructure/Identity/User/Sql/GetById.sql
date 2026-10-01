-- GetById.sql
SELECT Id, Username, PasswordHash, Role, IsActive
FROM auth.[User]
WHERE Id = @Id;