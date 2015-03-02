using UnityEngine;
using System.Collections;

public class RangeBehavior : BaseBehavior
{
    void OnTriggerEnter(Collider other)
    {
        // Just for test
        if (!GetMemory<bool>("isMoving"))
        {
            SetMemory<bool>("wasPushed", true);
        }
    }
}
