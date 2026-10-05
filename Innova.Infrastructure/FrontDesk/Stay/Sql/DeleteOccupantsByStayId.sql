-- DeleteOccupantsByStayId.sql - FrontDesk

DELETE
FROM frontdesk.StayOccupant
WHERE StayId = @StayId;