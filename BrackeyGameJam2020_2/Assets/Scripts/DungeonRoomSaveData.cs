using System;

[Serializable]
public class DungeonRoomSaveData
{
    public float[] gridPos;
    public int type;
    public bool doorUL, doorUR, doorBL, doorBR;

    public DungeonRoomSaveData(DungeonRoom room)
    {
        if (room == null)
        {
            return;
        }

        gridPos = new float[2] { room.gridPos.x, room.gridPos.y };
        type = (int)room.type;
        doorUL = room.doorUL;
        doorUR = room.doorUR;
        doorBL = room.doorBL;
        doorBR = room.doorBR;
    }

    public DungeonRoom ToDungeonRoom()
    {
        if (gridPos == null)
        {
            return null;
        }

        var room = new DungeonRoom(new UnityEngine.Vector2(gridPos[0], gridPos[1]), (RoomTypes)type);
        room.doorUL = doorUL;
        room.doorUR = doorUR;
        room.doorBL = doorBL;
        room.doorBR = doorBR;

        return room;
    }
}
