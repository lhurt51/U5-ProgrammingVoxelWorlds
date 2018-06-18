using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils {

    static int maxHeight = 150;
    static float smooth = 0.01f;
    static int octaves = 4;
    static float persistence = 0.5f;

    static float Map(float newMin, float newMax, float origMin, float origMax, float val)
    {
        return Mathf.Lerp(newMin, newMax, Mathf.InverseLerp(origMin, origMax, val));
    }

    static float fBM(float x, float z, int oct, float pers)
    {
        float total = 0.0f;
        float freq = 1.0f;
        float amp = 1.0f;
        float maxVal = 0.0f;

        for (int i = 0; i < oct; i++)
        {
            total += Mathf.PerlinNoise(x * freq, z * freq) * amp;
            maxVal += amp;
            amp *= pers;
            freq *= 2;
        }

        return total / maxVal;
    }

    public static int GenHeight(float x, float z)
    {
        return (int)Map(0.0f, maxHeight, 0.0f, 1.0f, fBM(x * smooth, z * smooth, octaves, persistence));
    }

    public static int GenStoneHeight(float x, float z)
    {
        return (int)Map(0.0f, maxHeight - 30, 0.0f, 1.0f, fBM(x * smooth * 2, z * smooth * 2, octaves + 1, persistence));
    }

}
