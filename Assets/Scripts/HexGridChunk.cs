using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridChunk : MonoBehaviour
{
    HexCell[] cells;
    HexMesh hexMesh;

    public void AddCell(int i, HexCell cell)
    {
        cells[i] = cell;
        cell.transform.SetParent(transform, false);
        cell.chunk = this;
    }

    public void Refresh()
    {
        hexMesh.Triangulate(cells);
    }

    public void Recolor()
    {
        hexMesh.Recolor(cells);
    }

    void Awake()
    {
        hexMesh = GetComponentInChildren<HexMesh>();

        cells = new HexCell[HexMetrics.chunkWidth * HexMetrics.chunkHeight];
    }
    void Start()
    {
        hexMesh.Triangulate(cells);
    }
}
