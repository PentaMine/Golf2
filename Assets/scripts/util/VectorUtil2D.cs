using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorUtil2D
{
    public static Vector2 moveToFirstQuadrant(Vector2 vector)
    {
        return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
    }

    public static Vector2 getPointInBetween(Vector2 vector1, Vector2 vector2)
    {
        return new Vector2((vector1.x + vector2.x) / 2, (vector1.y + vector2.y) / 2);
    }
}