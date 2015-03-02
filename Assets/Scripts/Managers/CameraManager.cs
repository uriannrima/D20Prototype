using UnityEngine;
using System.Collections;

public class CameraManager : BaseManager<CameraManager>
{
    /// <summary>
    /// Target to follow with the camera.
    /// </summary>
    public GameObject Target;

    /// <summary>
    /// Movement speed of the camera.
    /// </summary>
    public float MoveSpeed = 1f;

    /// <summary>
    /// Rotate speed of the camera.
    /// </summary>
    public float RotateSpeed = 1f;

    /// <summary>
    /// Zooming speed of the camera.
    /// </summary>
    public float ZoomSpeed = 1f;

    /// <summary>
    /// Pivot used to rotate, making this easier.
    /// </summary>
    private GameObject RotatePivot;

    void Start()
    {
        RotatePivot = GetChild("Rotate Pivot");
    }

    // Update is called once per frame
    void Update()
    {
        if (Target) this.transform.position = Target.transform.position;
    }

    /// <summary>
    /// Zoom out camera.
    /// </summary>
    /// <param name="zoomSpeed">Zoom speed.</param>
    public void ZoomOut(float zoomSpeed = 0)
    {
        if (zoomSpeed != 0)
        {
            Camera.main.transform.Translate(-Camera.main.transform.forward * zoomSpeed, Space.World);
        }
        else
        {
            Camera.main.transform.Translate(-Camera.main.transform.forward * ZoomSpeed, Space.World);
        }
    }

    /// <summary>
    /// Rotate on the X axis backwards.
    /// </summary>
    /// <param name="rotateSpeed">Rotation speed.</param>
    public void RotateBackwards(float rotateSpeed = 0)
    {
        if (rotateSpeed != 0)
        {
            RotatePivot.transform.Rotate(Vector3.right * rotateSpeed, -1f);
        }
        else
        {
            RotatePivot.transform.Rotate(Vector3.right * RotateSpeed, -1f);
        }
    }

    /// <summary>
    /// Zoom in camera.
    /// </summary>
    /// <param name="zoomSpeed">Zoom speed.</param>
    public void ZoomIn(float zoomSpeed = 0)
    {
        if (zoomSpeed != 0)
        {
            Camera.main.transform.Translate(Camera.main.transform.forward * zoomSpeed, Space.World);
        }
        else
        {
            Camera.main.transform.Translate(Camera.main.transform.forward * ZoomSpeed, Space.World);
        }
    }

    /// <summary>
    /// Rotate on the X axis forwards.
    /// </summary>
    /// <param name="rotateSpeed">Rotation speed.</param>
    public void RotateForwards(float rotateSpeed = 0)
    {
        if (rotateSpeed != 0)
        {
            RotatePivot.transform.Rotate(Vector3.right * rotateSpeed, 1f);
        }
        else
        {
            RotatePivot.transform.Rotate(Vector3.right * RotateSpeed, 1f);
        }
    }

    /// <summary>
    /// Rotate the camera anticlockwise.
    /// </summary>
    /// <param name="rotateSpeed">Rotation speed.</param>
    public void RotateRight(float rotateSpeed = 0)
    {
        if (rotateSpeed != 0)
        {
            this.transform.Rotate(Vector3.up, -rotateSpeed);
        }
        else
        {
            this.transform.Rotate(Vector3.up, -RotateSpeed);
        }
    }

    /// <summary>
    /// Rotate the camera clockwise.
    /// </summary>
    /// <param name="rotateSpeed">Rotation speed.</param>
    public void RotateLeft(float rotateSpeed = 0)
    {
        if (rotateSpeed != 0)
        {
            this.transform.Rotate(Vector3.up, rotateSpeed);
        }
        else
        {
            this.transform.Rotate(Vector3.up, RotateSpeed);
        }
    }

    /// <summary>
    /// Move the camera right.
    /// </summary>
    /// <param name="moveSpeed">Move speed.</param>
    public void MoveRight(float moveSpeed = 0)
    {
        if (moveSpeed != 0)
        {
            this.transform.Translate(Vector3.right * moveSpeed);
        }
        else
        {
            this.transform.Translate(Vector3.right * MoveSpeed);
        }
    }

    /// <summary>
    /// Move the camera left.
    /// </summary>
    /// <param name="moveSpeed">Move speed.</param>
    public void MoveLeft(float moveSpeed = 0)
    {
        if (moveSpeed != 0)
        {
            this.transform.Translate(-Vector3.right * moveSpeed);
        }
        else
        {
            this.transform.Translate(-Vector3.right * MoveSpeed);
        }
    }

    /// <summary>
    /// Move the camera forwards.
    /// </summary>
    /// <param name="moveSpeed">Move speed.</param>
    public void MoveForwards(float moveSpeed = 0)
    {
        if (moveSpeed != 0)
        {
            this.transform.Translate(Vector3.forward * moveSpeed);
        }
        else
        {
            this.transform.Translate(Vector3.forward * MoveSpeed);
        }
    }

    /// <summary>
    /// Move the camera backwards.
    /// </summary>
    /// <param name="moveSpeed">Move speed.</param>
    public void MoveBackwards(float moveSpeed = 0)
    {
        if (moveSpeed != 0)
        {
            this.transform.Translate(-Vector3.forward * moveSpeed);
        }
        else
        {
            this.transform.Translate(-Vector3.forward * MoveSpeed);
        }
    }
}
