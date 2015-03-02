using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BaseSelector<T> : BaseMonoBehaviour where T : class
{
    /// <summary>
    /// Handler to the selection.
    /// </summary>
    protected T SelectionHandler;

    /// <summary>
    /// Tags to be searched inside the Game Objects.
    /// </summary>
    public List<string> IncludeTags = new List<string>();

    /// <summary>
    /// All Selectables Game Objects by this Selector.
    /// </summary>
    public List<GameObject> Selectables = new List<GameObject>();

    void Awake()
    {
        SelectionHandler = GetInterfaceComponent<T>();

        // Find out all GameObjects with Tag.
        foreach (string tag in IncludeTags)
        {
            Selectables.AddRange(GameObject.FindGameObjectsWithTag(tag));
        }
    }
}
