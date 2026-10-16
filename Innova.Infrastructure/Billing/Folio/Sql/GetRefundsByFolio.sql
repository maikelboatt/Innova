-- GetRefundsByFolio.sql - Billing
-- Result set for loading a folio's refunds; also feeds GetFolioByIdQuery's statement.
SELECT Amount, Currency, Method, Reference, Reason, RefundedAt
FROM billing.Refund
WHERE FolioId = @FolioId
ORDER BY RefundedAt, Id;