using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Core;
using System;

public class MouseManager : BaseMonoBehaviour
{
    MultipleSelector mSelector;
    SingleSelector sSelector;
    SelectionBox selectionBox;

    void Awake()
    {
        mSelector = this.GetComponent<MultipleSelector>();
        sSelector = this.GetComponent<SingleSelector>();
        selectionBox = GetComponent<SelectionBox>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleDrag();
        HandleSelection();
    }

    /// <summary>
    /// Handle if mouse was drag
    /// </summary>
    void HandleDrag()
    {
        // Is selecting
        if (Input.GetMouseButton(0) && selectionBox.Dragged)
        {
            mSelector.HandleSelection(selectionBox.Selection, 0, true);
        }
        // Selected
        else if (Input.GetMouseButtonUp(0) && selectionBox.Dragged)
        {
            mSelector.HandleSelection(selectionBox.Selection, 0);
        }
    }

    void HandleSelection()
    {
        if (selectionBox.Dragged) return;

        if (Input.GetMouseButtonUp(0))
        {
            sSelector.HandleSelection(Input.mousePosition, 0);
        }

        if (Input.GetMouseButtonUp(1))
        {
            sSelector.HandleSelection(Input.mousePosition, 1);
        }
    }
}
