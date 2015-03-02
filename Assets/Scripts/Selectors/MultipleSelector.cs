using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultipleSelector : BaseSelector<IMultipleSelectionHandler>, IMultipleSelector
{
    /// <summary>
    /// Cache for selected objects.
    /// </summary>
    List<GameObject> SelectedObjects = new List<GameObject>();

    public void HandleSelection(Rect selectionBox, int buttonIndex, bool preSelection = false)
    {
        // Remove all unvalid object
        SelectedObjects = Selectables.FindAll(
            selectedObject =>
            {
                if (!selectedObject.renderer.isVisible) return false;

                Vector3 position = Camera.main.WorldToScreenPoint(selectedObject.transform.position);
                position.y = InvertY(position.y);

                return selectionBox.Contains(new Vector2(position.x, position.y));
            }
            );

        // Send it to the handler
        if (preSelection)
        {
            SelectionHandler.SelectingObjects(SelectedObjects, buttonIndex);
        }
        else
        {
            SelectionHandler.SelectedObjects(SelectedObjects, buttonIndex);
        }

        // Clear out references.
        SelectedObjects.Clear();
    }
}
