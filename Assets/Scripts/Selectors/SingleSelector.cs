using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SingleSelector : BaseSelector<ISingleSelectionHandler>, ISingleSelector
{
    /// <summary>
    /// Layers used to Raycast.
    /// </summary>
    public LayerMask RaycastLayer;

    /// <summary>
    /// Cache for selected object.
    /// </summary>
    GameObject SelectedObject = null;

    /// <summary>
    /// Raycast used to store the ray information.
    /// </summary>
    RaycastHit Hit;

    /// <summary>
    /// Handle single selection.
    /// </summary>
    /// <param name="selectionPosition">Point used to cast the ray from the camera.</param>
    /// <param name="buttonIndex">Index of the button used.</param>
    public void HandleSelection(Vector3 selectionPosition, int buttonIndex)
    {
        // Cast a ray and see if it hitted something.
        if (CastRay(selectionPosition))
        {
            // See if selected object is one of the selectables by this selector
            if (Selectables.Contains(Hit.transform.gameObject))
            {
                SelectedObject = Hit.transform.gameObject;
            }

            // Something was selected or not
            if (SelectedObject)
            {
                SelectionHandler.SelectObject(SelectedObject, buttonIndex);
            }
            else
            {
                SelectionHandler.SelectPosition(Hit.point, buttonIndex);
            }

            // Clear out Cache.
            SelectedObject = null;
        }
    }

    /// <summary>
    /// Uses Physics.Raycast to find out if hitted something using the main camera.
    /// </summary>
    /// <param name="selectionPosition">Position to be used from Screen Point to Ray.</param>
    /// <returns></returns>
    private bool CastRay(Vector3 selectionPosition, float distance = 100f)
    {
        return Physics.Raycast(Camera.main.ScreenPointToRay(selectionPosition), out Hit, distance, RaycastLayer);
    }
}
