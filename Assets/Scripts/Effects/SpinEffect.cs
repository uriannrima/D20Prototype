using UnityEngine;
using System.Collections;

public class SpinEffect : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public float Speed = 2f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * Speed * 10 * Time.deltaTime);
    }
}
