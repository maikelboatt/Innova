-- Insert.sql Room Inventory

INSERT INTO roominventory.Room(Id, RoomNumber, FloorLevel, FloorWing, RoomTypeId, Status)
VALUES (@Id, @RoomNumber, @FloorLevel, @FloorWing, @RoomTypeId, @Status);