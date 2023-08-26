using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public static class GameObjectUtility
{
    public static void DestroyAllChildren(this Transform transform)
    {
        var childs = transform.GetComponentsInChildren<Transform>();

        foreach(var child in childs)
        {
            if(child != transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }
}
