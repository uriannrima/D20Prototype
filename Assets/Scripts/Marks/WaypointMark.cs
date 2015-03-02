using UnityEngine;
using System.Collections;

public class WaypointMark : BaseMonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public GameObject Owner;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Owner)
        {
            gameObject.SetActive(false);
        }
    }

}
