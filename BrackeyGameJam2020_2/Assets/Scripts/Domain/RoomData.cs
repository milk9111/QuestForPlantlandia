using System;
using UnityEngine;

[Serializable]
public class RoomData
{
    public int[] pos;
    public Color color;

    public RoomData(int x, int y)
    {
        pos = new int[2] { x, y };
        color = Color.white;
    }

    public Vector2Int GetPos()
    {
        return new Vector2Int(pos[0], pos[1]);
    }
}
