using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class DungeonRoomStackState
{
    public IDictionary<Vector2, DungeonRoomManagerSave> rooms;
    public UnitSave playerUnit;

    public DungeonRoomStackState(IDictionary<Vector2, DungeonRoomManagerSave> rooms, UnitSave playerUnit)
    {
        this.rooms = rooms;
        this.playerUnit = playerUnit;
    }

    public DungeonRoomStackState(DungeonRoomStackStateSaveData save)
    {
        rooms = save.rooms.ToDictionary(kv => new Vector2(kv.Key[0], kv.Key[1]), kv => kv.Value.ToDungeonRoomManagerSave());
        playerUnit = save.playerUnit;
    }
}
