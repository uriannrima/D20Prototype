using UnityEngine;
using System.Collections;

public class PulseEffect : MonoBehaviour
{
    public float Speed = 0.1f;

    public float Minimum = 0.15f;

    public float Maximum = 0.20f;

    // Update is called once per frame
    void Update()
    {
        float scale = Mathf.PingPong(Time.time * Speed, Maximum - Minimum) + Minimum;
        transform.localScale = new Vector3(scale, transform.localScale.y, scale);
    }
}
