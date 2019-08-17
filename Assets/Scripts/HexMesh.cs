using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    Mesh hexMesh;
    MeshCollider meshCollider;
    List<Vector3> vertices;
    List<int> triangles;
    List<Color> colors;


    void Awake()
    {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        hexMesh.name = "Hex Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();
    }

    public void Recolor(HexCell[] cells)
    {
        colors.Clear();

        for (int i = 0; i < cells.Length; i++)
        {
            Recolor(cells[i]);
        }

        hexMesh.colors = colors.ToArray();
    }

    public void Recolor(HexCell cell)
    {
        Color highlight = Color.black;
        if (cell.IsHighlighted())
        {
            highlight = new Color(0.7f, 0.7f, 0.7f);
        }
        Color color = cell.color + highlight;
        Color border = cell.borderColor + highlight;
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            // Hex Core
            AddTriangleColor(color);
            // Outer Hex
            for (int i = 0; i < 4; i++)
            {
                AddTriangleColor(color);
            }

            //AddTriangleColor(color, border, border);
            //AddTriangleColor(color, border, border);
            //AddTriangleColor(color, border, color);
            //AddTriangleColor(color, border, border);

            // Border
            HexCell neighbor = cell.GetNeighbor(d);
            if (neighbor == null || neighbor.borderColor == cell.borderColor)
            {
                for (int i = 0; i < 8; i++)
                {
                    AddTriangleColor(color);
                }
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    AddTriangleColor(border);
                }
            }
            /*
            AddTriangleColor(color, border, border);
            AddTriangleColor(color, border, color);
            AddTriangleColor(color, border, border);
            AddTriangleColor(color, border, border);
            AddTriangleColor(color, color, border);
            AddTriangleColor(color, border, border);
            AddTriangleColor(color, border, color);
            AddTriangleColor(color, border, border);
            */
            /*
            AddTriangleColor(border, color, color);
            AddTriangleColor(border, color, border);
            AddTriangleColor(border, color, color);
            AddTriangleColor(border, color, color);
            AddTriangleColor(border, border, color);
            AddTriangleColor(border, color, color);
            AddTriangleColor(border, color, border);
            AddTriangleColor(border, color, color);
            */


        }
    }

    public void Triangulate(HexCell[] cells)
    {
        hexMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        colors.Clear();

        for (int i = 0; i < cells.Length; i++)
        {
            Triangulate(cells[i]);
            Recolor(cells[i]);
        }


        hexMesh.vertices = vertices.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.colors = colors.ToArray();
        hexMesh.RecalculateNormals();
        meshCollider.sharedMesh = hexMesh;
    }

    void Triangulate(HexCell cell)
    {
        Vector3 center = cell.transform.localPosition;
        center.y = 0f;
        Vector3 height = new Vector3(0f, cell.height, 0f);
        float average = cell.height;
        int cellCount = 1;
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            if (cell.GetNeighbor(d) != null)
            {
                cellCount++;
                average += cell.GetNeighbor(d).height;
            }
        }
        average /= cellCount;

        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            // Heights
            Vector3 baseHeight = new Vector3(0f, 0f, 0f);
            Vector3 prevMidHeight = baseHeight;
            Vector3 firstMidHeight = baseHeight;
            Vector3 nextMidHeight = baseHeight;
            Vector3 secondMidHeight = baseHeight;
            Vector3 midHeight = baseHeight;
            Vector3 firstOuterHeight = baseHeight;
            Vector3 secondOuterHeight = baseHeight;
            Vector3 firstConnectorHeight = baseHeight;
            Vector3 secondConnectorHeight = baseHeight;
            Vector3 innerMidHeight = baseHeight;
            float dHeight = cell.GetNeighbor(d) == null ? 0f : cell.GetNeighbor(d).height;
            float dCCWHeight = cell.GetNeighbor(d.CounterClockwise(1)) == null ? 0f : cell.GetNeighbor(d.CounterClockwise(1)).height;
            float dCWHeight = cell.GetNeighbor(d.Clockwise(1)) == null ? 0f : cell.GetNeighbor(d.Clockwise(1)).height;
            float bridgeProportion = cell.bridgeProportion;
            if (cell.GetNeighbor(d) != null)
            {
                midHeight.y = (cell.height + dHeight) / 2f;
                innerMidHeight = (height + midHeight) / 2f;
                bridgeProportion += cell.GetNeighbor(d).bridgeProportion;
                bridgeProportion /= 2f;
            }
            if (cell.GetNeighbor(d) != null && cell.GetNeighbor(d.CounterClockwise(1)) != null)
            {
                prevMidHeight.y = cell.height + dCCWHeight;
                prevMidHeight.y /= 2f;
                firstMidHeight.y = cell.height + dHeight + dCCWHeight;
                firstMidHeight.y /= 3f;
                firstOuterHeight = (height + midHeight + prevMidHeight) / 3f;
                firstConnectorHeight = (firstMidHeight + midHeight) / 2f;
            }
            if (cell.GetNeighbor(d) != null && cell.GetNeighbor(d.Clockwise(1)) != null)
            {
                nextMidHeight.y = cell.height + dCWHeight;
                nextMidHeight.y /= 2f;
                secondMidHeight.y = cell.height + dHeight + dCWHeight;
                secondMidHeight.y /= 3f;
                secondOuterHeight = (height + midHeight + nextMidHeight) / 3f;
                secondConnectorHeight = (secondMidHeight + midHeight) / 2f;
            }

            // Vertices
            Vector3 coreCorner1 = center + HexMetrics.FirstCorner(d) * HexMetrics.coreMultiplier + height;
            Vector3 coreCorner2 = center + HexMetrics.SecondCorner(d) * HexMetrics.coreMultiplier + height;
            Vector3 outerCorner1 = center + HexMetrics.FirstCorner(d) + firstOuterHeight;
            Vector3 outerCorner2 = center + HexMetrics.SecondCorner(d) + secondOuterHeight;
            Vector3 borderCorner1 = center + (HexMetrics.FirstCorner(d) * HexMetrics.borderMultiplier) + firstMidHeight;
            Vector3 borderCorner2 = center + (HexMetrics.SecondCorner(d) * HexMetrics.borderMultiplier) + secondMidHeight;
            Vector3 bridgeConnector1 = center + (HexMetrics.FirstCorner(d) + HexMetrics.BorderMiddle(d)) + firstConnectorHeight;
            Vector3 bridgeConnector2 = center + (HexMetrics.SecondCorner(d) + HexMetrics.BorderMiddle(d)) + secondConnectorHeight;
            Vector3 bridgeInner1 = center + (HexMetrics.FirstCorner(d) * cell.bridgeProportion + HexMetrics.SecondCorner(d) * (1 - cell.bridgeProportion)) + innerMidHeight;
            Vector3 bridgeInner2 = center + (HexMetrics.SecondCorner(d) * cell.bridgeProportion + HexMetrics.FirstCorner(d) * (1 - cell.bridgeProportion)) + innerMidHeight;
            Vector3 bridgeOuter1 = center + (HexMetrics.FirstCorner(d) * bridgeProportion + HexMetrics.SecondCorner(d) * (1 - bridgeProportion)) + HexMetrics.BorderMiddle(d) + midHeight;
            Vector3 bridgeOuter2 = center + (HexMetrics.SecondCorner(d) * bridgeProportion + HexMetrics.FirstCorner(d) * (1 - bridgeProportion)) + HexMetrics.BorderMiddle(d) + midHeight;
            //bridgeInner1.y = bridgeInner2.y = (cell.height + midHeight.y) / 2f;
            //bridgeOuter1.y = bridgeOuter2.y = midHeight.y;

            // Core Hex
            AddTriangle(
                center + height,
                coreCorner1,
                coreCorner2
                );

            // Outer Hex
            AddTriangle(
                coreCorner1,
                outerCorner1,
                bridgeInner1
                );
            AddTriangle(
                coreCorner1,
                bridgeInner1,
                bridgeInner2
                );
            AddTriangle(
                coreCorner1,
                bridgeInner2,
                coreCorner2
                );
            AddTriangle(
                coreCorner2,
                bridgeInner2,
                outerCorner2
                );

            // Border
            AddTriangle(
                outerCorner1,
                borderCorner1,
                bridgeConnector1
            );
            AddTriangle(
                outerCorner1,
                bridgeConnector1,
                bridgeInner1
            );
            AddTriangle(
                bridgeInner1,
                bridgeConnector1,
                bridgeOuter1
            );
            AddTriangle(
                bridgeInner1,
                bridgeOuter1,
                bridgeOuter2
            );
            AddTriangle(
                bridgeInner2,
                bridgeInner1,
                bridgeOuter2
            );
            AddTriangle(
                bridgeInner2,
                bridgeOuter2,
                bridgeConnector2
            );
            AddTriangle(
                bridgeInner2,
                bridgeConnector2,
                outerCorner2
            );
            AddTriangle(
                outerCorner2,
                bridgeConnector2,
                borderCorner2
            );
        }
    }

    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    void AddTriangleColor(Color color)
    {
        for (int i = 0; i < 3; i++)
        {
            colors.Add(color);
        }
    }

    void AddTriangleColor(Color color1, Color color2, Color color3)
    {
        colors.Add(color1);
        colors.Add(color2);
        colors.Add(color3);
    }
}
