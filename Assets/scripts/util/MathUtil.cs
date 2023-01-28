using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtil
{
    public static float RoundToMultiple(float value, float n)
    {
        return Mathf.Round(value / n) * n;
        
    }

    public static float radiansToDegrees(float radians)
    {
        return radians * Mathf.Rad2Deg;
    }
}