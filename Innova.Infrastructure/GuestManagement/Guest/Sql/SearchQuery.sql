-- SearchQuery.sql

SELECT g.Id     AS GuestId,
       LTRIM(RTRIM(
               CONCAT(
                       g.FirstName,
                       CASE
                           WHEN NULLIF(LTRIM(RTRIM(g.MiddleName)), '') IS NOT NULL
                               THEN ' ' + LTRIM(RTRIM(g.MiddleName))
                           ELSE ''
                           END,
                       CASE
                           WHEN NULLIF(LTRIM(RTRIM(g.LastName)), '') IS NOT NULL
                               THEN ' ' + LTRIM(RTRIM(g.LastName))
                           ELSE ''
                           END
               )
             )) AS FullName,
       g.PhoneNumber,
       g.IsActive
FROM guestmgmt.Guest g
WHERE (
    @NameContains IS NULL
        OR
    (
        g.FirstName LIKE '%' + @NameContains + '%'
            OR g.MiddleName LIKE '%' + @NameContains + '%'
            OR g.LastName LIKE '%' + @NameContains + '%'
            OR CONCAT(
                       g.FirstName, ' ',
                       COALESCE(g.MiddleName + ' ', ''),
                       g.LastName
               ) LIKE '%' + @NameContains + '%'
        )
    )
  AND (
    @PhoneContains IS NULL
        OR g.PhoneNumber LIKE '%' + @PhoneContains + '%'
    )
  AND (
    @IsActive IS NULL
        OR g.IsActive = @IsActive
    )
ORDER BY g.LastName,
         g.FirstName,
         g.MiddleName;