using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour
{
    public Color[] colors;
    public HexGrid hexGrid;
    public GameObject featurePrefab;

    private Color activeColor;

    void Awake()
    {
        SelectColor(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            HandleInput(0);
        }
        else
        {
            HandleInput(1);
        }
    }

    void HandleInput(int inputType)
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            if (inputType == 0)
            {
                //hexGrid.ColorCell(hit.point, activeColor);
                hexGrid.AddFeature(hit.point, featurePrefab);
            }
            else
            {
                hexGrid.HighlightCell(hit.point);
            }
        }
    }

    public void SelectColor(int index)
    {
        activeColor = colors[index];
    }
}
