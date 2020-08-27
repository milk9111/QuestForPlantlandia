using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DungeonSaveData
{
    public float[] worldSize;
    public DungeonRoomSaveData[,] dungeonRooms;
    public List<float[]> eventProbabilities;
    public List<float[]> takenPositions;
    public int gridSizeX, gridSizeY, numberOfRooms;
    public Dictionary<float[], DungeonRoomManagerSaveData> drawnRooms;
    public float[] partyPosition;
    public bool isNew;
    public int level;

    public DungeonSaveData(DungeonSave d)
    {
        worldSize = new float[2] { d.worldSize.x, d.worldSize.y };
        dungeonRooms = new DungeonRoomSaveData[d.dungeonRooms.GetLength(0), d.dungeonRooms.GetLength(1)];
        for (var x = 0; x < d.dungeonRooms.GetLength(0); x++)
        {
            for (var y = 0; y < d.dungeonRooms.GetLength(1); y++)
            {
                dungeonRooms[x, y] = new DungeonRoomSaveData(d.dungeonRooms[x, y]);
            }
        }

        eventProbabilities = new List<float[]>();
        foreach(var v in d.eventProbabilities)
        {
            eventProbabilities.Add(new float[2] { v.x, v.y });
        }

        takenPositions = new List<float[]>();
        foreach (var v in d.takenPositions)
        {
            takenPositions.Add(new float[2] { v.x, v.y });
        }

        gridSizeX = d.gridSizeX;
        gridSizeY = d.gridSizeY;
        numberOfRooms = d.numberOfRooms;
        drawnRooms = new Dictionary<float[], DungeonRoomManagerSaveData>();
        foreach(var kv in d.drawnRooms)
        {
            drawnRooms.Add(new float[2] { kv.Key.x, kv.Key.y }, new DungeonRoomManagerSaveData(kv.Value));
        }

        partyPosition = new float[3] { d.partyPosition.x, d.partyPosition.y, d.partyPosition.z };
        isNew = d.isNew;
        level = d.level;
    }

    public DungeonSave ToDungeonSave()
    {
        var d = new DungeonSave();
        d.worldSize = new UnityEngine.Vector2(worldSize[0], worldSize[1]);
        d.dungeonRooms = new DungeonRoom[dungeonRooms.GetLength(0), dungeonRooms.GetLength(1)];
        for (var x = 0; x < dungeonRooms.GetLength(0); x++)
        {
            for (var y = 0; y < dungeonRooms.GetLength(1); y++)
            {
                d.dungeonRooms[x, y] = dungeonRooms[x, y].ToDungeonRoom();
            }
        }

        d.eventProbabilities = new List<UnityEngine.Vector2>();
        foreach (var v in eventProbabilities)
        {
            d.eventProbabilities.Add(new UnityEngine.Vector2(v[0], v[1]));
        }

        d.takenPositions = new List<UnityEngine.Vector2>();
        foreach (var v in takenPositions)
        {
            d.takenPositions.Add(new UnityEngine.Vector2(v[0], v[1]));
        }

        d.gridSizeX = gridSizeX;
        d.gridSizeY = gridSizeY;
        d.numberOfRooms = numberOfRooms;
        d.drawnRooms = new Dictionary<UnityEngine.Vector2, DungeonRoomManagerSave>();
        foreach (var kv in drawnRooms)
        {
            d.drawnRooms.Add(new UnityEngine.Vector2(kv.Key[0], kv.Key[1]), kv.Value.ToDungeonRoomManagerSave());
        }

        d.partyPosition = new UnityEngine.Vector3(partyPosition[0], partyPosition[1], partyPosition[2]);
        d.isNew = isNew;
        d.level = level;

        return d;
    }
}
