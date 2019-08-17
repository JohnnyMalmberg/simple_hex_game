using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HexDirection
{
    NE, E, SE, SW, W, NW
}

public static class HexDirectionMethods
{
    public static HexDirection Opposite(this HexDirection direction)
    {
        return direction.Clockwise(3);
    }

    public static HexDirection Clockwise(this HexDirection direction, int dist)
    {
        return (HexDirection)(((int)direction + dist) % 6);
    }
    public static HexDirection CounterClockwise(this HexDirection direction, int dist)
    {
        return direction.Clockwise(6 - dist);
    }
}
