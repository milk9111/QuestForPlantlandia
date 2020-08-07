using System;

[Serializable]
public class DungeonRoomManagerSaveData
{
    public DungeonRoomSaveData room;
    public float[] gridPos;
    public float[] color;
    public bool isShowing;
    public bool isFound;
    public bool isCurrent;
    public RoomEventSave roomEvent;
    public float[] position;
    public int roomType;

    public DungeonRoomManagerSaveData(DungeonRoomManagerSave man)
    {
        room = new DungeonRoomSaveData(man.room);
        gridPos = new float[2] { man.gridPos.x, man.gridPos.y };
        color = new float[4] { man.color.r, man.color.g, man.color.b, man.color.a };
        isShowing = man.isShowing;
        isFound = man.isFound;
        isCurrent = man.isCurrent;
        roomEvent = man.roomEvent;
        position = new float[3] { man.position.x, man.position.y, man.position.z };
        roomType = (int)man.roomType;
    }

    public DungeonRoomManagerSave ToDungeonRoomManagerSave()
    {
        return new DungeonRoomManagerSave
        {
            room = room.ToDungeonRoom(),
            gridPos = new UnityEngine.Vector2(gridPos[0], gridPos[1]),
            color = new UnityEngine.Color(color[0], color[1], color[2], color[3]),
            isShowing = isShowing,
            isFound = isFound,
            isCurrent = isCurrent,
            roomEvent = roomEvent,
            position = new UnityEngine.Vector3(position[0], position[1], position[2]),
            roomType = (RoomTypes)roomType
        };
    }
}
