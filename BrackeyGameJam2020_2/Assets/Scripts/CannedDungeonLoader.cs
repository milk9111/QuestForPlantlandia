using System;
using System.IO;
using Microsoft.VisualBasic;
using UnityEngine;

public class CannedDungeonLoader
{
    public static Tuple<Vector2Int, RoomData[,]> Load(string filename)
    {
        StreamReader sr = new StreamReader($"Assets/CannedLevels/{filename}");
        var sizeLine = sr.ReadLine().Split(',');
        var size = new Vector2Int(Convert.ToInt32(sizeLine[0]), Convert.ToInt32(sizeLine[1]));

        var rooms = new RoomData[size.x, size.y];

        string[] row;
        string line;
        var y = size.y - 1;
        while ((line = sr.ReadLine()) != null)
        {
            row = line.Split(',');
            for (var x = 0; x < size.x; x++)
            {
                var ch = row[x].Trim();
                if (string.IsNullOrEmpty(ch))
                {
                    continue;
                }

                rooms[x, y] = new RoomData(x, y);
            }

            y--;
        }

        return new Tuple<Vector2Int, RoomData[,]> (size, rooms);
    }
}
