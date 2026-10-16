-- InsertRefund.sql - Billing
INSERT INTO billing.Refund (FolioId, Amount, Currency, Method, Reference, Reason, RefundedAt)
VALUES (@FolioId, @Amount, @Currency, @Method, @Reference, @Reason, @RefundedAt);