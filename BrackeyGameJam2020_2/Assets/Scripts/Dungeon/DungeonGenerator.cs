using System;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public Vector2Int mapSize;
    public NoiseLayer noise;
    public GameObject tile;
    public Transform dungeon;

    public bool useCannedLevel;
    public string cannedLevel;

    [Range(0f, 1f)]
    public float noiseThreshold;

    private RoomData[,] rooms;

    void Start()
    {
        if (!useCannedLevel)
        {
            BuildDungeon();
        } else
        {
            var result = CannedDungeonLoader.Load(cannedLevel);
            mapSize = result.Item1;
            rooms = result.Item2;
        }

        CleanupDungeon();
        RenderDungeon();
    }

    private void BuildDungeon()
    {
        rooms = new RoomData[mapSize.x, mapSize.y];
        for (var x = 0; x < mapSize.x; x++)
        {
            for (var y = 0; y < mapSize.y; y++)
            {
                var noiseResult = noise.Noise(x, y);
                if (noiseResult > noiseThreshold)
                {
                    rooms[x, y] = new RoomData(x, y);
                }
            }
        }
    }

    private void CleanupDungeon()
    {
        for (var x = 0; x < mapSize.x; x++)
        {
            for (var y = 0; y < mapSize.y; y++)
            {
                var room = rooms[x, y];
                if (room == null)
                {
                    continue;
                }

                if (HasUpperLeftOnly(room, rooms))
                {
                    Debug.Log("Had upper left only!");
                    rooms[x, y + 1] = new RoomData(x, y + 1);
                    rooms[x, y + 1].color = Color.red;
                    room.color = Color.black;
                }
            }
        }
    }

    private void RenderDungeon()
    {
        for (var x = 0; x < mapSize.x; x++)
        {
            for (var y = 0; y < mapSize.y; y++)
            {
                var room = rooms[x, y];
                if (room != null)
                {
                    var tileObj = Instantiate(tile, new Vector3(5 * x, 5 * y), Quaternion.identity);
                    tileObj.transform.parent = dungeon;
                    if (room.color != Color.white)
                    {
                        tileObj.GetComponent<SpriteRenderer>().color = room.color;
                    }
                }
            }
        }
    }

    private bool HasUpperLeftOnly(RoomData room, RoomData[,] rooms)
    {
        var pos = room.GetPos();

        return pos.x - 1 >= 0 && pos.y + 1 < mapSize.y && rooms[pos.x - 1, pos.y + 1] != null && rooms[pos.x, pos.y + 1] == null && rooms[pos.x - 1, pos.y] == null;
    }

    private bool HasUpperRight(RoomData room, RoomData[,] rooms)
    {
        var pos = room.GetPos();

        return pos.x + 1 < rooms.Length && pos.y - 1 >= 0 && rooms[pos.x + 1, pos.y - 1] != null;
    }

    private bool HasBottomLeft(RoomData room, RoomData[,] rooms)
    {
        var pos = room.GetPos();

        return pos.x - 1 >= 0 && pos.y + 1 < rooms.GetLength(pos.x) && rooms[pos.x - 1, pos.y + 1] != null;
    }

    private bool HasBottomRight(RoomData room, RoomData[,] rooms)
    {
        var pos = room.GetPos();

        return pos.x + 1 < rooms.Length && pos.y + 1 < rooms.GetLength(pos.x) && rooms[pos.x + 1, pos.y + 1] != null;
    }
}
