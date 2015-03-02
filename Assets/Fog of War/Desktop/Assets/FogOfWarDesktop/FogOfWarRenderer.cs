using UnityEngine;
using System.Collections;

public class FogOfWarRenderer : MonoBehaviour
{
    [SerializeField]
    FogOfWar fogOfWar = null;

    void Start()
    {

    }

    void Update()
    {
        if (fogOfWar != null && fogOfWar.enabled && fogOfWar.gameObject.activeInHierarchy)
        {
            renderer.enabled = fogOfWar.IsRevealed(transform.position);
        }
    }
}