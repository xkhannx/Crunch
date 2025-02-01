using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectTileButton : MonoBehaviour
{
    public bool selected;
    MapEditor brush;
    [SerializeField] MapEditor.BrushType buttonType;
    private void Start()
    {
        brush = FindObjectOfType<MapEditor>();
    }
    private void OnMouseDown()
    {
        selected = !selected;
        if (selected)
            brush.SetBrush(buttonType);
        else
            brush.SetBrush(MapEditor.BrushType.Empty);
    }
}
