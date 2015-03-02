using UnityEngine;
using System.Collections;

public class BaseManager<T> : BaseMonoBehaviour where T : BaseManager<T>
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null) _instance = GameObject.FindObjectOfType<T>();
            return _instance;
        }
    }
}
