-- InsertPayment.sql - Billing

INSERT INTO billing.Payment
(FolioId,
 Amount,
 Currency,
 Method,
 Reference,
 ReceivedAt)
VALUES (@FolioId,
        @Amount,
        @Currency,
        @Method,
        @Reference,
        @ReceivedAt);