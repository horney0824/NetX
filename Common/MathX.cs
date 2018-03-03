using System.Collections;
using System.Collections.Generic;
using System;

public static partial class MathX
{
    public static int Sign(int value)
    {
        if (value == 0)
            return 0;

        return value / Math.Abs(value);
    }

    public static int Sign(float value)
    {
        if (value > 0)
            return 1;
        else if (value < 0)
            return -1;

        return 0;
    }

    public static short DegToShort(float deg)
    {
        return (short)(deg / 180.0f * 32767);
    }

    public static float ShortToDeg(short shortAngle)
    {
        return (float)shortAngle / 32767.0f * 180.0f;
    }

    public static int Clamp(int value, int min, int max)
    {
        if (value <= min)
            value = min;

        if (value >= max)
            value = max;

        return value;
    }

    public static float Min(float v0, float v1)
    {
        return Math.Min(v0, v1);
    }

    public static float Max(float v0, float v1)
    {
        return Math.Max(v0, v1);
    }

    public static float Min(float v0, float v1, float v2)
    {
        return Math.Min(v0, Math.Min(v1, v2));
    }

    public static float Max(float v0, float v1, float v2)
    {
        return Math.Max(v0, Math.Max(v1, v2));
    }

    public static float Min(float v0, float v1, float v2, float v3)
    {
        return Math.Min(v0, Math.Min(v1, Math.Min(v2, v3)));
    }

    public static float Max(float v0, float v1, float v2, float v3)
    {
        return Math.Max(v0, Math.Max(v1, Math.Max(v2, v3)));
    }
}
