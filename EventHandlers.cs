using Exiled.API.Features;
using Exiled.API.Enums;
using System.Linq;
using Exiled.API.Features.Doors;

namespace NukeRoomRadiation
{
    public class EventHandlers
    {
        private static Plugin plugin;
        public EventHandlers(Plugin P) => plugin = P;

        public void OnGenerated()
        {
            // Find the D-Class room
            RoomType[][] lightPath = new RoomType[][]
            {
                new RoomType[] { RoomType.LczTCross },
                new RoomType[] { RoomType.LczPlants },
                new RoomType[] { RoomType.LczCrossing },
                new RoomType[] { RoomType.LczCheckpointA, RoomType.LczCheckpointB }
            };
            Room classDSpawn = Room.List.Where(room => room.Type == RoomType.LczClassDSpawn).FirstOrDefault();

            (bool isConnected, RoomType lastRoomType) = CheckPath(classDSpawn, lightPath);

            if (!isConnected)
            {
                Log.Info("The D-Class room is not connected to the specified path.");
                Round.Restart();
                return;
            }

            Room heavyRoom = Room.List.Where(room => room.Type == (lastRoomType == RoomType.LczCheckpointA ? RoomType.HczElevatorA : RoomType.HczElevatorB)).FirstOrDefault();

            Log.Info("Checking heavy path");

            RoomType[][] heavyPathStart = new RoomType[][]
            {
                new RoomType[] { RoomType.HczTCross, RoomType.HczArmory },
                new RoomType[] { RoomType.HczStraight },
                new RoomType[] { RoomType.HczCrossing, RoomType.HczTCross, RoomType.HczArmory },
                new RoomType[] { RoomType.Hcz096 }
            };

            RoomType[][] heavyPathFull = new RoomType[][]
            {
                new RoomType[] { RoomType.HczTCross, RoomType.HczArmory },
                new RoomType[] { RoomType.HczStraight },
                new RoomType[] { RoomType.HczCrossing, RoomType.HczTCross, RoomType.HczArmory},
                new RoomType[] { RoomType.HczStraight },
                new RoomType[] { RoomType.HczTCross, RoomType.HczArmory },
                new RoomType[] { RoomType.HczEzCheckpointA, RoomType.HczEzCheckpointB }
            };

            (bool heavyPathStartFound, _) = CheckPath(heavyRoom, heavyPathStart);
            (bool heavyPathEndFound, RoomType lastHeavyRoomType) = CheckPath(heavyRoom, heavyPathFull);

            if (!heavyPathStartFound || !heavyPathEndFound)
            {
                Log.Info("The D-Class room is not connected to the specified path.");
                Round.Restart();
                return;
            }

            Room entranceRoom = Room.List.Where(room => room.Type == lastHeavyRoomType).FirstOrDefault();

            Log.Info("Checking entrance path");
            RoomType[][] entrancePath = new RoomType[][]
            {
                new RoomType[] { RoomType.EzCheckpointHallway },
                new RoomType[] { RoomType.EzStraight },
                new RoomType[] { RoomType.EzCrossing },
                new RoomType[] { RoomType.EzGateB }
            };

            (bool entrancePathFound, _) = CheckPath(entranceRoom, entrancePath);

            if (entrancePathFound)
            {
                Log.Info("Found seed: " + Map.Seed);
                // Perform any desired actions when the D-Class room is connected to the specified path
            }
            else
            {
                Log.Info("The D-Class room is not connected to the specified path.");
                Round.Restart();
                // Perform any desired actions when the D-Class room is not connected to the specified path
            }
        }

        private (bool, RoomType) CheckPath(Room startRoom, RoomType[][] desiredRoomTypes)
        {
            return CheckPathRecursive(startRoom, desiredRoomTypes, 0);
        }

        private (bool, RoomType) CheckPathRecursive(Room currentRoom, RoomType[][] desiredRoomTypes, int currentIndex)
        {
            if (currentIndex >= desiredRoomTypes.Length)
            {
                // All desired room types have been found
                return (true, desiredRoomTypes[currentIndex - 1][0]);
            }

            RoomType[] desiredRoomType = desiredRoomTypes[currentIndex];

            foreach (Door door in currentRoom.Doors)
            {
                if (door.GameObject != null)
                {
                    Room nextRoom = door.Rooms.Where((Room room) => room != currentRoom).FirstOrDefault();
                    if (nextRoom != null && desiredRoomType.Contains(nextRoom.Type))
                    {
                        Log.Info($"Found {nextRoom.Type} room.");
                        (bool isPathFound, RoomType lastRoomType) = CheckPathRecursive(nextRoom, desiredRoomTypes, currentIndex + 1);
                        if (isPathFound)
                            return (true, lastRoomType);
                    }
                }
            }

            return (false, RoomType.Unknown);
        }
    }
}