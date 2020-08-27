using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator
{
    private float _seed;

    public NoiseGenerator()
    {
        RandomSeed();
    }

    public NoiseGenerator(float seed)
    {
        Seed(seed);
    }

    public void Seed(float seed)
    {
        while (seed > 1)
        {
            seed = seed / 10;
        }

        _seed = seed;
    }

    public void RandomSeed()
    {
        _seed = Random.Range(0f, 10f);
    }

    public float GetSeed()
    {
        return _seed;
    }

    public float Generate(float x, float y)
    {
        return Mathf.PerlinNoise(_seed + x, _seed + y);
    }
}
