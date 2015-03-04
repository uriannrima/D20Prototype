using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseMonoBehaviour : MonoBehaviour
{
    /// <summary>
    /// Find all children inside this Game Object.
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetChildren()
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
        }
        return children;
    }

    /// <summary>
    /// Find all children inside this Game Object using Child Name as filter.
    /// </summary>
    /// <param name="childName"></param>
    /// <returns></returns>
    public List<GameObject> GetChildren(string childName)
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in transform)
        {
            if (child.name == childName) children.Add(child.gameObject);
        }
        return children;
    }

    /// <summary>
    /// Return first appearance of Child Game Object with Child Name.
    /// </summary>
    /// <param name="childName"></param>
    /// <returns></returns>
    public GameObject GetChild(string childName)
    {
        foreach (Transform child in transform)
        {
            if (child.name == childName) return child.gameObject;
        }

        return null;
    }

    /// <summary>
    /// Find Component which implements interface I.
    /// </summary>
    /// <typeparam name="I">Type of Interface.</typeparam>
    /// <returns>Component as I.</returns>
    public I GetInterfaceComponent<I>() where I : class
    {
        return GetComponent(typeof(I)) as I;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="I"></typeparam>
    /// <returns></returns>
    public static List<I> FindObjectsOfInterface<I>() where I : class
    {
        MonoBehaviour[] monoBehaviours = FindObjectsOfType<MonoBehaviour>();
        List<I> list = new List<I>();
        foreach (MonoBehaviour behaviour in monoBehaviours)
        {
            I component = behaviour.GetComponent(typeof(I)) as I;
            if (component != null)
            {
                list.Add(component);
            }
        }
        return list;
    }

    protected void ChangeColor(Color color)
    {
        this.GetComponent<Renderer>().material.color = color;
    }

    public static void Log(object message)
    {
        Debug.Log(message);
    }

    protected float InvertY(float y)
    {
        return Screen.height - y;
    }
}

public static class GameObjectExt
{
    /// <summary>
    /// Return first appearance of Child Game Object with Child Name.
    /// </summary>
    /// <param name="childName"></param>
    /// <returns></returns>
    public static GameObject GetChild(this GameObject gameObject, string childName)
    {
        foreach (Transform child in gameObject.transform)
        {
            if (child.name == childName) return child.gameObject;
        }

        return null;
    }

    /// <summary>
    /// Find all children inside this Game Object using Child Name as filter.
    /// </summary>
    /// <param name="childName"></param>
    /// <returns></returns>
    public static List<GameObject> GetChildren(this GameObject gameObject, string childName)
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in gameObject.transform)
        {
            if (child.name == childName) children.Add(child.gameObject);
        }
        return children;
    }

    /// <summary>
    /// Return first component of type T, from the first child that has it.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static T GetComponentInChild<T>(this GameObject gameObject) where T : Component
    {
        foreach (Transform child in gameObject.transform)
        {
            if (child.GetComponent<T>()) return child.GetComponent<T>();
        }

        return null;
    }

}

public static class ComponentExt
{
    /// <summary>
    /// Return first component of type T, from the first child that has it.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static T GetComponentInChild<T>(this Component component) where T : Component
    {
        foreach (Transform child in component.gameObject.transform)
        {
            if (child.GetComponent<T>()) return child.GetComponent<T>();
        }

        return null;
    }
}
