using UnityEngine;
using System.Collections;

public class TutorialGUI : MonoBehaviour
{
    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 200, 30), "WASD/Arrows - Move Camera");
        GUI.Label(new Rect(0, 15, 150, 30), "QE - Rotate Camera");
        GUI.Label(new Rect(0, 30, 150, 30), "Scroll - Zoom in and out");
        GUI.Label(new Rect(0, 45, 300, 30), "Shift + Scroll - Rotate on X Axis");

        string message = (EdgeManager.Instance.Edging) ? "Enabled" : "Disabled";
        GUI.Label(new Rect(0, 60, 300, 30), "X - Toggles Edging: " + message);

        GUI.Label(new Rect(0, 90, 300, 30), "Left Click - Simple Selection");
        GUI.Label(new Rect(0, 105, 300, 30), "Ctrl + Left Click - Multiple Selection");
        GUI.Label(new Rect(0, 120, 300, 30), "Left Drag - Multiple Selection");
        GUI.Label(new Rect(0, 135, 300, 30), "Right Click - Move");
        GUI.Label(new Rect(0, 150, 300, 30), "Spacebar - Focus on Selected");

        message = (!Physics.GetIgnoreLayerCollision(11, 11)) ? "Enabled" : "Disabled";
        GUI.Label(new Rect(0, 180, 300, 30), "C - Toggles Character Collision: " + message);
    }
}
