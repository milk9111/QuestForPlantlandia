using System;
using UnityEngine;

[Serializable]
public class NoiseLayer
{
    public float power = 1f;
    public Vector2 offset;
    public float scale = 1f;

    private NoiseGenerator _noise;

    public void SetNoiseGenerator(NoiseGenerator noise)
    {
        _noise = noise;
    }

    public float Noise(float x, float y)
    {
        if (_noise == null)
        {
            _noise = new NoiseGenerator();
        }

        var xCoord = x * scale + offset.x;
        var yCoord = y * scale + offset.y;

        return (_noise.Generate(xCoord, yCoord)) * power;
    }
}
