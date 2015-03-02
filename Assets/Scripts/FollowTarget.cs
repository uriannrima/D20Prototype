using UnityEngine;
using System.Collections;

public class FollowTarget : BaseMonoBehaviour
{
    /// <summary>
    /// Target to follow with the camera.
    /// </summary>
    public GameObject Target;

    private GameObject RotatePivot;

    // Use this for initialization
    void Start()
    {
        RotatePivot = GetChild("Rotate Pivot");
    }

    // Update is called once per frame
    void Update()
    {
        if (Target) this.transform.position = Target.transform.position;
        HandleMovement();
        HandleRotation();
    }

    private void HandleRotation()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            this.transform.Rotate(Vector3.up, 1f);
        }
        if (Input.GetKey(KeyCode.E))
        {
            this.transform.Rotate(Vector3.up, -1f);
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                RotatePivot.transform.Rotate(Vector3.right, 1f);
            }
            else
            {
                Camera.main.transform.Translate(Camera.main.transform.forward, Space.World);
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                RotatePivot.transform.Rotate(Vector3.right, -1f);
            }
            else
            {
                Camera.main.transform.Translate(-Camera.main.transform.forward, Space.World);
            }
        }
    }

    private void HandleMovement()
    {
        if (Input.GetKey(KeyCode.A))
        {
            this.transform.Translate(-Vector3.right);
        }
        if (Input.GetKey(KeyCode.D))
        {
            this.transform.Translate(Vector3.right);
        }

        if (Input.GetKey(KeyCode.W))
        {
            this.transform.Translate(Vector3.forward);
        }
        if (Input.GetKey(KeyCode.S))
        {
            this.transform.Translate(-Vector3.forward);
        }
    }
}
