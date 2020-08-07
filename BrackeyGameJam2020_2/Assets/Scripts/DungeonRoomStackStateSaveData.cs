using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class DungeonRoomStackStateSaveData
{
    public Dictionary<float[], DungeonRoomManagerSaveData> rooms;
    public UnitSave playerUnit;

    public DungeonRoomStackStateSaveData(DungeonRoomStackState s)
    {
        rooms = s.rooms.ToDictionary(kv => new float[2] { kv.Key.x, kv.Key.y }, kv => new DungeonRoomManagerSaveData(kv.Value));
        playerUnit = s.playerUnit;
    }
}
