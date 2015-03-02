using UnityEngine;
using System.Collections;
using RAIN.Core;

public class ConfigureMemory : MonoBehaviour
{
    AIRig AIRig;

    // Use this for initialization
    void Start()
    {
        AIRig = GetComponent<AIRig>();
        if (AIRig)
        {
            AIRig.AI.WorkingMemory.SetItem<Vector3>("movePosition", transform.position);
        }
    }
}
