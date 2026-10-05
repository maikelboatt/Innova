-- Update.sql - FrontDesk

UPDATE frontdesk.Stay
SET ActualCheckOut = @ActualCheckOut
WHERE Id = @Id;