using UnityEngine;
using System.Collections;

public class KeyboardManager : BaseManager<KeyboardManager>
{
    void Update()
    {
        HandleCamera();
        HandleDebugMode();
    }

    bool toggleCollision = false;
    private void HandleDebugMode()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            toggleCollision = !toggleCollision;
            Physics.IgnoreLayerCollision(11, 11, toggleCollision);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            EdgeManager.Instance.Edging = !EdgeManager.Instance.Edging;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Camera.main.GetComponent<FogOfWarEffect>().enabled = !Camera.main.GetComponent<FogOfWarEffect>().enabled;
        }

    }

    /// <summary>
    /// Handle camera functions.
    /// </summary>
    void HandleCamera()
    {
        HandleCameraFocus();
        HandleCameraMovement();
        HandleCameraRotation();
    }

    /// <summary>
    /// Handle camera movement.
    /// </summary>
    void HandleCameraMovement()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            CameraManager.Instance.MoveLeft();
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            CameraManager.Instance.MoveRight();
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            CameraManager.Instance.MoveForwards();
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            CameraManager.Instance.MoveBackwards();
        }
    }

    /// <summary>
    /// Handle camera rotation.
    /// </summary>
    private void HandleCameraRotation()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            CameraManager.Instance.RotateLeft();
        }
        if (Input.GetKey(KeyCode.E))
        {
            CameraManager.Instance.RotateRight();
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                CameraManager.Instance.RotateForwards();
            }
            else
            {
                CameraManager.Instance.ZoomIn();
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                CameraManager.Instance.RotateBackwards();
            }
            else
            {
                CameraManager.Instance.ZoomOut();
            }
        }
    }

    /// <summary>
    /// Handle camera focus.
    /// </summary>
    private void HandleCameraFocus()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (SelectionManager.Instance.Units.Count > 0)
            {
                CameraManager.Instance.Target = SelectionManager.Instance.Units[0];
            }
        }
        else
        {
            CameraManager.Instance.Target = null;
        }
    }

}
