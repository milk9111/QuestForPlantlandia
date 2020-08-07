using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DungeonSave
{
    public Vector2 worldSize = new Vector2(4, 4);
    public DungeonRoom[,] dungeonRooms;

    public List<Vector2> eventProbabilities;

    public IList<Vector2> takenPositions = new List<Vector2>();

    public int gridSizeX, gridSizeY, numberOfRooms = 20;

    public IDictionary<Vector2, DungeonRoomManagerSave> drawnRooms = new Dictionary<Vector2, DungeonRoomManagerSave>();

    public Vector3 partyPosition;

    public bool isNew = true;

    public int level;
}
