using UnityEngine;
using System.Collections;

public class LOSEnemy : LOSEntity
{
    // Update is called once per frame
    void Update()
    {
        // Being rendered
        if (GetComponent<Renderer>().enabled)
        {
            // Hidden or fogged.
            if (RevealState == RevealStates.Hidden ||
                RevealState == RevealStates.Fogged)
            {
                GetComponent<Renderer>().enabled = false;
            }
        }
        else
        {
            if (RevealState == RevealStates.Unfogged)
            {
                GetComponent<Renderer>().enabled = true;
            }
        }
    }
}
