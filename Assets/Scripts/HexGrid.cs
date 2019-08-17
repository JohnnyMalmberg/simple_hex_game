using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    // Inputs
    public int chunkCountX = 6;
    public int chunkCountZ = 6;
    public Color defaultColor = Color.black;
    public Color defaultBorderColor = Color.black;

    // Prefabs
    public HexGridChunk chunkPrefab;
    public HexCell cellPrefab;
    public Text cellLabelPrefab;

    HexGridChunk[] chunks;
    HexCell[] cells;
    Canvas gridCanvas;

    int width;
    int height;

    float noiseOffset;

    void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();

        chunks = new HexGridChunk[chunkCountX * chunkCountZ];
        width = chunkCountX * HexMetrics.chunkWidth;
        height = chunkCountZ * HexMetrics.chunkHeight;
        noiseOffset = Random.value * 10000f;

        CreateChunks();
        CreateCells();
    }

    void CreateChunks()
    {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        for (int z = 0, i = 0; z < chunkCountZ; z++)
        {
            for (int x = 0; x < chunkCountX; x++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }
    }

    void CreateCells()
    {
        cells = new HexCell[width * height];
        for (int z = 0, i = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    void CreateCell(int x, int z, int i)
    {
        Vector3 position = CellPosition(x, z);

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        //cell.transform.SetParent(transform, false);
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

        cell.height = CellHeight(x, z, position);
        position.y = cell.height;
        cell.transform.localPosition = position;

        cell.bridgeProportion = HexMetrics.bridgeProportion + (Random.value * 0.2f - 0.1f);

        cell.color = defaultColor;// / 3 + Color.white * 0.66f;
        cell.borderColor = defaultBorderColor;

        if (x > 0)
        {
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        }
        if (z > 0)
        {
            if ((z & 1) == 0)
            {
                cell.SetNeighbor(HexDirection.SE, cells[i - width]);
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - width]);
                if (x < width - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
                }
            }
        }

        bool showCoordinates = false;
        if (showCoordinates)
        {
            Text cellLabel = Instantiate<Text>(cellLabelPrefab);
            //cellLabel.rectTransform.SetParent(gridCanvas.transform, false);
            cellLabel.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
            cellLabel.text = cell.coordinates.ToString();
        }

        AddToChunk(x, z, cell);
    }

    void AddToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.chunkWidth;
        int chunkZ = z / HexMetrics.chunkHeight;

        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];
        int localX = x - (chunkX * HexMetrics.chunkWidth);
        int localZ = z - (chunkZ * HexMetrics.chunkHeight);

        chunk.AddCell(localX + localZ * HexMetrics.chunkWidth, cell);

    }


    public void ColorCell(Vector3 position, Color color)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        if (index < 0 || index > width * height)
        {
            return;
        }
        HexCell cell = cells[index];
        cell.color = Color.black;
        cell.borderColor = color;
        cell.Recolor();
    }

    public void AddFeature(Vector3 position, GameObject featurePrefab)
    {
        GameObject feature = Instantiate<GameObject>(featurePrefab);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        if (index < 0 || index > width * height)
        {
            return;
        }
        HexCell cell = cells[index];
        cell.AddFeature(feature);
    }

    public void HighlightCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        if (index < 0 || index > width * height)
        {
            return;
        }
        HexCell cell = cells[index];
        if (cell.height > 3f)
        {
            cell.Highlight();
            cell.Recolor();
        }
    }

    float CellHeight(int x, int z, Vector3 position)
    {
        float worldBorder = 16f;
        float cellHeight = Random.value * 2.5f;
        float noiseX = position.x + noiseOffset;
        float noiseZ = position.z + noiseOffset;
        cellHeight += Mathf.PerlinNoise(noiseX / 60f, noiseZ / 60f) * 15f;
        cellHeight += Mathf.PerlinNoise(noiseX / 10f, noiseZ / 10f) * 7.5f;
        cellHeight += Mathf.PerlinNoise(noiseX / 5f, noiseZ / 5f) * 3.5f;
        cellHeight += Mathf.PerlinNoise(noiseX / 50f, noiseZ / 50f) * 20f;
        cellHeight += Mathf.PerlinNoise(noiseX, noiseZ) * 5f;
        cellHeight -= 20f;
        if (x < worldBorder)
        {
            cellHeight *= x / worldBorder;
        }
        if (z < worldBorder)
        {
            cellHeight *= z / worldBorder;
        }
        if (x > width - worldBorder - 1)
        {
            cellHeight *= (width - x - 1) / worldBorder;
        }
        if (z > height - worldBorder - 1)
        {
            cellHeight *= (height - z - 1) / worldBorder;
        }
        return cellHeight;
    }

    Vector3 CellPosition(int x, int z)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f * HexMetrics.borderMultiplier);
        position.y = 0f;
        position.z = (z * HexMetrics.borderMultiplier) * (HexMetrics.outerRadius * 1.5f);
        return position;
    }
}
