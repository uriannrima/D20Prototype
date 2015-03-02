using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IMultipleSelectionHandler
{
    /// <summary>
    /// Method called when multiple game objects are selected on screen.
    /// </summary>
    /// <param name="selectedObject">Selected GameObjects on screen.</param>
    /// <param name="mouseButton">Button that was used to select.</param>
    void SelectedObjects(List<GameObject> selectedObjects, int buttonIndex);

    /// <summary>
    /// Method called when multiple game objects are still beeing selected on screen.
    /// </summary>
    /// <param name="selectedObject">Selected GameObjects on screen.</param>
    /// <param name="mouseButton">Button that was used to select.</param>
    void SelectingObjects(List<GameObject> selectedObjects, int buttonIndex);
}
