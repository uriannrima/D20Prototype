using UnityEngine;
using System.Collections;
using RAIN.Core;

public class BaseBehavior : BaseMonoBehaviour 
{
    /// <summary>
    /// RAIN Rig.
    /// </summary>
    protected AIRig AIRig;

    protected virtual void Awake()
    {
        AIRig = GetComponentInChildren<AIRig>();
    }

    /// <summary>
    /// Set item in working memory.
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    /// <param name="itemName">Memory item name.</param>
    /// <param name="value">Value to memory item.</param>
    protected void SetMemory<T>(string itemName, T value)
    {
        if (!AIRig) return;

        AIRig.AI.WorkingMemory.SetItem<T>(itemName, value);
    }

    /// <summary>
    /// Get item in working memory.
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    /// <param name="itemName">Memory item name.</param>
    /// <returns>Value of item memory as type T.</returns>
    protected T GetMemory<T>(string itemName)
    {
        return AIRig.AI.WorkingMemory.GetItem<T>(itemName);
    }

}
