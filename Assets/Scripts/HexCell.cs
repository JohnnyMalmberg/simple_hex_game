using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;
    public Color color;
    public Color borderColor;
    int highlighted;
    public float height;
    public float bridgeProportion;
    [SerializeField]
    HexCell[] neighbors;
    public HexGridChunk chunk;
    public GameObject[] features;

    void Awake()
    {
        features = new GameObject[6];
        highlighted = 0;
    }

    public void SetFeature(int index, GameObject feature)
    {
        feature.transform.localPosition = this.transform.position + HexMetrics.FirstCorner((HexDirection)index) * HexMetrics.coreMultiplier * (0.4f + Random.value * 0.2f);
        feature.transform.rotation = Quaternion.Euler(0f, Random.value * 360f, 0f);
        if (features[index] != null)
        {
            Object.Destroy(features[index]);
        }
        features[index] = feature;
    }

    public void AddFeature(GameObject feature)
    {
        for (int index = 0; index < 6; index++)
        {
            if (features[index] == null)
            {
                this.SetFeature(index, feature);
                return;
            }
        }
        this.SetFeature(0, feature);
    }

    public void Highlight()
    {
        highlighted = 2;
    }

    public bool IsHighlighted()
    {
        return highlighted > 0;
    }

    void Update()
    {
        if (highlighted > 0)
        {
            highlighted--;
            if (highlighted == 0)
            {
                Recolor();
            }
        }
    }

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    public void Refresh()
    {
        chunk.Refresh();
    }

    public void Recolor()
    {
        chunk.Recolor();
        for (int i = 0; i < neighbors.Length; i++)
        {
            HexCell neighbor = neighbors[i];
            if (neighbor != null && neighbor.chunk != chunk)
            {
                neighbor.chunk.Recolor();
            }
        }
    }
}
