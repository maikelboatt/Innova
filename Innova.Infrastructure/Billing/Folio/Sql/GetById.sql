-- GetById.sql - Billing

SELECT f.Id,
       f.OwnerType,
       f.OwnerId,
       f.Currency,
       f.Status
FROM billing.Folio f
WHERE f.Id = @FolioId;

SELECT c.Id,
       c.FolioId,
       c.Amount,
       c.Currency,
       c.Category,
       c.Description,
       c.PostedAt
FROM billing.Charge c
WHERE c.FolioId = @FolioId
ORDER BY c.PostedAt, c.Id;

SELECT p.Id,
       p.FolioId,
       p.Amount,
       p.Currency,
       p.Method,
       p.Reference,
       p.ReceivedAt
FROM billing.Payment p
WHERE p.FolioId = @FolioId
ORDER BY p.ReceivedAt, p.Id;

SELECT a.Id,
       a.FolioId,
       a.Amount,
       a.Currency,
       a.Type,
       a.Reason,
       a.PostedAt
FROM billing.Adjustment a
WHERE a.FolioId = @FolioId
ORDER BY a.PostedAt, a.Id;