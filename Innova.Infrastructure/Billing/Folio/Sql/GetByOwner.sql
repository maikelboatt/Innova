-- GetByOwner.sql - Billing

SELECT f.Id,
       f.OwnerType,
       f.OwnerId,
       f.Currency,
       f.Status
FROM billing.Folio f
WHERE f.OwnerType = @OwnerType
  AND f.OwnerId = @OwnerId
ORDER BY f.Id;

SELECT c.Id,
       c.FolioId,
       c.Amount,
       c.Currency,
       c.Category,
       c.Description,
       c.PostedAt
FROM billing.Charge c
         INNER JOIN billing.Folio f
                    ON f.Id = c.FolioId
WHERE f.OwnerType = @OwnerType
  AND f.OwnerId = @OwnerId
ORDER BY c.FolioId, c.PostedAt, c.Id;

SELECT p.Id,
       p.FolioId,
       p.Amount,
       p.Currency,
       p.Method,
       p.Reference,
       p.ReceivedAt
FROM billing.Payment p
         INNER JOIN billing.Folio f
                    ON f.Id = p.FolioId
WHERE f.OwnerType = @OwnerType
  AND f.OwnerId = @OwnerId
ORDER BY p.FolioId, p.ReceivedAt, p.Id;

SELECT a.Id,
       a.FolioId,
       a.Amount,
       a.Currency,
       a.Type,
       a.Reason,
       a.PostedAt
FROM billing.Adjustment a
         INNER JOIN billing.Folio f
                    ON f.Id = a.FolioId
WHERE f.OwnerType = @OwnerType
  AND f.OwnerId = @OwnerId
ORDER BY a.FolioId, a.PostedAt, a.Id;