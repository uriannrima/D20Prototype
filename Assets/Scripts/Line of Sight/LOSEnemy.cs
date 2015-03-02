using UnityEngine;
using System.Collections;

public class LOSEnemy : LOSEntity
{
    // Update is called once per frame
    void Update()
    {
        // Being rendered
        if (renderer.enabled)
        {
            // Hidden or fogged.
            if (RevealState == RevealStates.Hidden ||
                RevealState == RevealStates.Fogged)
            {
                renderer.enabled = false;
            }
        }
        else
        {
            if (RevealState == RevealStates.Unfogged)
            {
                renderer.enabled = true;
            }
        }
    }
}
