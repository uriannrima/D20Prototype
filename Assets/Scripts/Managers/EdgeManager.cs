using UnityEngine;
using System.Collections;

public class EdgeManager : BaseManager<EdgeManager>
{
    /// <summary>
    /// Edge Margin.
    /// </summary>
    public float EdgeMargin = 25;

    /// <summary>
    /// 
    /// </summary>
    public bool Edging = false;

    // Update is called once per frame
    void Update()
    {
        if (!Edging) return;

        // Check if on the top edge
        if (Input.mousePosition.y >= Screen.height - EdgeMargin)
        {
            CameraManager.Instance.MoveForwards();
        }
        else if (Input.mousePosition.y <= EdgeMargin)
        {
            CameraManager.Instance.MoveBackwards();
        }

        // Check if on the right edge
        if (Input.mousePosition.x >= Screen.width - EdgeMargin)
        {
            // Move the camera
            CameraManager.Instance.MoveRight();
        }
        else if (Input.mousePosition.x <= EdgeMargin)
        {
            CameraManager.Instance.MoveLeft();
        }
    }
}
