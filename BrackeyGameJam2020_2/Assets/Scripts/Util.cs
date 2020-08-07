using System;
using UnityEngine;

public static class Util
{
    public static Vector3 Round(Vector3 vector)
    {
        var x = Math.Floor(vector.x);
        if (vector.x % 1 >= 0.5f)
        {
            x = Math.Ceiling(vector.x);
        }

        var y = Math.Floor(vector.y);
        if (vector.y % 1 >= 0.5f)
        {
            y = Math.Ceiling(vector.y);
        }

        var z = Math.Floor(vector.z);
        if (vector.z % 1 >= 0.5f)
        {
            z = Math.Ceiling(vector.z);
        }

        return new Vector3((float)x, (float)y, (float)z);
    }
}
