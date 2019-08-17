using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexMetrics
{
    public const float outerRadius = 4f; // 10 / 7

    public const float innerRadius = outerRadius * 0.866025404f;
    // Portion of the hex that is the flat core:
    public const float coreMultiplier = 0.75f;
    // How much larger should the hex be once the border is added:
    public const float borderMultiplier = 1.2f; // 1.05 / 1.2

    public const float bridgeProportion = (1f + 0.5f) / 2f;
    public const float bridgeProportionInverse = 1f - bridgeProportion;

    public const int chunkHeight = 8;
    public const int chunkWidth = 8;

    public static Vector3[] corners = {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius)
    };

    public static Vector3 FirstCorner(HexDirection direction)
    {
        return corners[(int)direction];
    }

    public static Vector3 SecondCorner(HexDirection direction)
    {
        return corners[((int)direction + 1) % 6];
    }

    public static Vector3 BorderMiddle(HexDirection direction)
    {
        return (borderMultiplier - 1f) * (FirstCorner(direction) + SecondCorner(direction)) / 2f;
    }

}

