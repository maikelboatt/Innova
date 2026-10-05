-- GetOccupantsByStayId.sql - FrontDesk

SELECT so.StayId,
       so.GuestId
FROM frontdesk.StayOccupant so
WHERE so.StayId = @StayId
ORDER BY so.GuestId;