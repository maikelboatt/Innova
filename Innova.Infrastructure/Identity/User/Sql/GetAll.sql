-- GetAll.sql
SELECT Id AS UserId, Username, Role, IsActive
FROM auth.[User]
ORDER BY Username;