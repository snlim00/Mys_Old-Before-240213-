using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectList : MonoBehaviour
{
    [SerializeField] private GameObject contents;
    [SerializeField] private GameObject objectListPref;

    public static string unnamed = "! Unnamed !";

    public void RefreshList(List<string> objectList)
    {
        contents.transform.DestroyAllChildren();

        foreach(var name in objectList)
        {
            CreateCharacterList(name);
        }
    }

    private GameObject CreateCharacterList(string name)
    {
        GameObject go = Instantiate(objectListPref);

        go.transform.SetParent(contents.transform);
        go.transform.localScale = Vector2.one;

        Button btn = go.GetComponent<Button>();

        btn.SetButtonText(name);

        if(string.IsNullOrEmpty(name))
        {
            btn.SetButtonText(unnamed);
        }

        return go;
    }
}