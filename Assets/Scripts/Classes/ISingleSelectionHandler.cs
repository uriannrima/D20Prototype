using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Class can handle single selection.
/// </summary>
public interface ISingleSelectionHandler
{
    /// <summary>
    /// Method called when a game object is selected on screen.
    /// </summary>
    /// <param name="selectedObject">Selected GameObject on screen.</param>
    /// <param name="mouseButton">Button that was used to select.</param>
    void SelectObject(GameObject selectedObject, int buttonIndex);

    /// <summary>
    /// Method called when a position is selected on screen.
    /// </summary>
    /// <param name="selectedPosition">Selected position on screen.</param>
    /// <param name="mouseButton">Button that was used to select.</param>
    void SelectPosition(Vector3 selectedPosition, int buttonIndex);
}