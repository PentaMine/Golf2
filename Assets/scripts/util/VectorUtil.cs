using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorUtil
{
    public static Vector3 getNormalisedEuler(Vector3 euler)
    {
        euler.x %= 360;
        euler.y %= 360;
        euler.z %= 360;

        euler.x += euler.x < 0 ? 360 : 0;
        euler.y += euler.y < 0 ? 360 : 0;
        euler.z += euler.z < 0 ? 360 : 0;
        return euler;
    }

    public static bool IsWithinTolerance(Vector3 vector, Vector3 tolerance)
    {
        vector = moveToFirstQuadrant(vector);
        return vector.x <= tolerance.x && vector.y <= tolerance.y && vector.z < tolerance.z;
    }

    public static Vector3 moveToFirstQuadrant(Vector3 vector)
    {
        return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
    }
}
